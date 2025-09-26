using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.ParseModes;
using Newtonsoft.Json.Linq;

namespace BlendJson.DataSources
{
    public class FileDataSource : IDataSource
    {
        public string Path { get; set; }

        public string WorkDir { get; set; }

        public Encoding Encoding { get; set; }

        public async Task<(JToken, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context,
            CancellationToken token)
        {

            var (bestSearchPath, newDataSource) = await GetActualPathAsync(lastDataSource, mode, context, token);

            newDataSource.WorkDir = System.IO.Path.GetDirectoryName(bestSearchPath);

            var fsProvider = context.FsProvider;

            switch (mode)
            {
                case LoadMode.Json:
                    return (JToken.Parse(await fsProvider.LoadTextFileAsync(bestSearchPath, newDataSource.Encoding, token)), newDataSource);
                case LoadMode.Text:
                    return (new JValue(await fsProvider.LoadTextFileAsync(bestSearchPath, newDataSource.Encoding, token)), newDataSource);
                case LoadMode.Bin:
                    return (JToken.FromObject(await fsProvider.LoadBinFileAsync(bestSearchPath, token)), newDataSource);
                case LoadMode.LargeBin:
                    return (JToken.FromObject(await fsProvider.LoadLargeBinFileAsync(bestSearchPath, token)), newDataSource);
                case LoadMode.Lines:
                    var lines = (await fsProvider.LoadTextFileAsync(bestSearchPath, newDataSource.Encoding, token))
                        .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(q => (object)q.Trim('\r')).ToArray();
                    return (new JArray(lines), newDataSource);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<(string Path, FileDataSource NewDataSource)> GetActualPathAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context, CancellationToken token)
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



            string bestSearchPath = null;

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

            if (bestSearchPath == null && context.ParseMode is MergeMode)
                throw new FileNotFoundException($"File not found: {Path}");

            return (bestSearchPath, newDataSource);
        }
    }
}
