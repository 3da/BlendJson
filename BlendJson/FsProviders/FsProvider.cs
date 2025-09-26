using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.DataSources;

namespace BlendJson.FsProviders
{
    public class FsProvider : IFsProvider
    {
        public Task<bool> FileExistsAsync(string path)
        {
            return Task.FromResult(File.Exists(path));
        }

        public async Task<string> LoadTextFileAsync(string path, Encoding encoding, CancellationToken token)
        {
            return await File.ReadAllTextAsync(path, encoding, token);
        }

        public async Task<byte[]> LoadBinFileAsync(string path, CancellationToken token)
        {
            return await File.ReadAllBytesAsync(path, token);
        }

        public async Task<byte[][]> LoadLargeBinFileAsync(string path, CancellationToken token)
        {
            using var stream = File.OpenRead(path);
            return await StreamUtils.LoadLargeBytesFromStreamAsync(stream, token);
        }

        public IEnumerable<string> GetSearchPaths(string path, string workDir, LoadMode mode)
        {
            var searchPaths = new List<string>
            {
                path
            };

            if (mode == LoadMode.Json)
                searchPaths.Add(path + ".json");

            if (workDir != null)
            {
                searchPaths.Insert(0, Path.Combine(workDir, path));
                if (mode == LoadMode.Json)
                    searchPaths.Insert(1, Path.Combine(workDir, path + ".json"));
            }

            return searchPaths;
        }



    }
}
