using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlendJson.DataSources
{
    public class ZipDataSource : IDataSource
    {
        [JsonIgnore]
        public ZipArchive ZipArchive { get; set; }

        public string Path { get; set; }

        public Encoding Encoding { get; set; }

        public async Task<(JToken, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode,
            ParseContext context, CancellationToken token)
        {
            if (lastDataSource is ZipDataSource f)
            {
                ZipArchive = ZipArchive ?? f.ZipArchive;
                Encoding = Encoding ?? f.Encoding;
            }


            if (Encoding == null)
                Encoding = Encoding.UTF8;

            ZipArchiveEntry entry;

            if (ZipArchive == null)
            {
                var stream = File.OpenRead(Path);
                ZipArchive = new ZipArchive(stream);

                entry = ZipArchive.GetEntry("Main.json");
            }
            else
                entry = ZipArchive.GetEntry(Path);

            await using (var stream = entry.Open())
            {
                string text = null;
                if (mode == LoadMode.Json || mode == LoadMode.Text)
                    text = await new StreamReader(stream, Encoding).ReadToEndAsync();
                switch (mode)
                {
                    case LoadMode.Json:
                        return (JToken.Parse(text), this);
                    case LoadMode.Text:
                        return (new JValue(text), this);
                    case LoadMode.Bin:
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream, token);
                            return (JToken.FromObject(memoryStream.ToArray()), this);
                        }
                    case LoadMode.LargeBin:
                        return (JToken.FromObject(await StreamUtils.LoadLargeBytesFromStreamAsync(stream, token)), this);
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
                        return (new JArray(lines), this);
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }



        }
    }
}
