using BlendJson.TypeResolving;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.DataSources
{
    public class ZipDataSource : IDataSource
    {
        [JsonIgnore]
        public ZipArchive? ZipArchive { get; set; }

        public required string Path { get; init; }

        [JsonConverter(typeof(EncodingConverter))]
        public Encoding? Encoding { get; set; }

        public async Task<(JsonNode, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode,
            ParseContext context, CancellationToken token)
        {
            if (lastDataSource is ZipDataSource f)
            {
                ZipArchive = ZipArchive ?? f.ZipArchive;
                Encoding = Encoding ?? f.Encoding;
            }


            if (Encoding == null)
                Encoding = Encoding.UTF8;

            ZipArchiveEntry? entry;

            if (ZipArchive == null)
            {
                var stream = File.OpenRead(Path);
                ZipArchive = new ZipArchive(stream);

                entry = ZipArchive.GetEntry("Main.json");
            }
            else
                entry = ZipArchive.GetEntry(Path);

            if (entry == null)
                throw new SettingsException($"Cannot find entry \"{Path}\" in zip archive");

            await using (var stream = entry.Open())
            {
                string? text = null;
                if (mode == LoadMode.Json || mode == LoadMode.Text)
                    text = await new StreamReader(stream, Encoding).ReadToEndAsync(token);
                switch (mode)
                {
                    case LoadMode.Json:
                        return (JsonNode.Parse(text!), this);
                    case LoadMode.Text:
                        return (JsonValue.Create(text), this);
                    case LoadMode.Bin:
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream, token);
                            return (JsonSerializer.SerializeToNode(memoryStream.ToArray()), this);
                        }
                    case LoadMode.LargeBin:
                        return (JsonSerializer.SerializeToNode(await StreamUtils.LoadLargeBytesFromStreamAsync(stream, token)), this);
                    case LoadMode.Lines:
                        var reader = new StreamReader(stream, Encoding);
                        var lines = new List<string>();
                        while (true)
                        {
                            var line = await reader.ReadLineAsync(token);
                            if (line == null)
                                break;

                            lines.Add(line);

                        }
                        return (JsonSerializer.SerializeToNode(lines), this);
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }



        }
    }
}
