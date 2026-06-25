using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.TypeResolving;

namespace BlendJson.DataSources
{
    public class FileDataSource : IDataSource
    {
        public required string Path { get; init; }

        public string? WorkDir { get; set; }

        [JsonConverter(typeof(EncodingConverter))]
        public Encoding? Encoding { get; set; }

        public async Task<(JsonNode, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context,
            CancellationToken token)
        {
            var (bestSearchPath, newDataSource) = await GetActualPathAsync(lastDataSource, mode, context, token);


            newDataSource.WorkDir = System.IO.Path.GetDirectoryName(bestSearchPath);

            var fsProvider = context.FsProvider;

            switch (mode)
            {
                case LoadMode.Json:
                    return (JsonNode.Parse(await fsProvider.LoadTextFileAsync(bestSearchPath!, newDataSource.Encoding!, token)), newDataSource);
                case LoadMode.Text:
                    return (JsonValue.Create(await fsProvider.LoadTextFileAsync(bestSearchPath!, newDataSource.Encoding!, token)), newDataSource);
                case LoadMode.Bin:
                    return (JsonSerializer.SerializeToNode(await fsProvider.LoadBinFileAsync(bestSearchPath!, token)), newDataSource);
                case LoadMode.LargeBin:
                    return (JsonSerializer.SerializeToNode(await fsProvider.LoadLargeBinFileAsync(bestSearchPath!, token)), newDataSource);
                case LoadMode.Lines:
                    var lines = (await fsProvider.LoadTextFileAsync(bestSearchPath!, newDataSource.Encoding!, token))
                        .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(q => (object)q.Trim('\r')).ToArray();
                    return (JsonSerializer.SerializeToNode(lines), newDataSource);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(string? Path, FileDataSource NewDataSource)> GetActualPathAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context, CancellationToken token)
        {
            var newDataSource = new FileDataSource()
            {
                Encoding = Encoding,
                Path = Path,
                WorkDir = WorkDir
            };

            newDataSource.WorkDir = WorkDir ?? lastDataSource.WorkDir;
            newDataSource.Encoding = Encoding ?? lastDataSource.Encoding;

            if (newDataSource.Encoding == null)
                newDataSource.Encoding = context.DefaultEncoding;

            var fsProvider = context.FsProvider;

            var searchPaths = fsProvider.GetSearchPaths(newDataSource.Path, newDataSource.WorkDir, mode);



            string? bestSearchPath = null;

            if (context.CheckFileExist)
            {
                foreach (var searchPath in searchPaths)
                {
                    if (await fsProvider.FileExistsAsync(searchPath))
                    {
                        bestSearchPath = searchPath;
                        break;
                    }
                }
            }
            else
                bestSearchPath = searchPaths.FirstOrDefault();

            if (bestSearchPath == null && context.MergeMode != null)
                throw new FileNotFoundException($"File not found: {Path}");

            return (bestSearchPath, newDataSource);
        }
    }
}
