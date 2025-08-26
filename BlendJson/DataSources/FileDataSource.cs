using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            var newDataSource = new FileDataSource()
            {
                Encoding = Encoding,
                Path = Path,
                WorkDir = WorkDir
            };
            if (lastDataSource is FileDataSource f)
            {
                newDataSource.WorkDir = WorkDir ?? f.WorkDir;
                newDataSource.Encoding = Encoding ?? f.Encoding;
            }

            if (newDataSource.Encoding == null)
                newDataSource.Encoding = Encoding.UTF8;


            var searchPaths = new List<string>
            {
                newDataSource.Path
            };

            if (mode == LoadMode.Json)
                searchPaths.Add(newDataSource.Path + ".json");

            if (newDataSource.WorkDir != null)
            {
                searchPaths.Insert(0, System.IO.Path.Combine(newDataSource.WorkDir, Path));
                if (mode == LoadMode.Json)
                    searchPaths.Insert(1, System.IO.Path.Combine(newDataSource.WorkDir, Path + ".json"));
            }

            string bestSearchPath = null;

            var fsProvider = context.FsProvider;

            foreach (var searchPath in searchPaths)
            {
                if (fsProvider.FileExists(searchPath))
                {
                    bestSearchPath = searchPath;
                    break;
                }
            }
            if (bestSearchPath == null)
                throw new FileNotFoundException($"File not found: {Path}");

            newDataSource.WorkDir = System.IO.Path.GetDirectoryName(bestSearchPath);

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
    }
}
