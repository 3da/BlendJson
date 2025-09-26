using BlendJson.DataSources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.FsProviders
{
    public interface IFsProvider
    {
        Task<bool> FileExistsAsync(string path);
        Task<string> LoadTextFileAsync(string path, Encoding encoding, CancellationToken token);
        Task<byte[]> LoadBinFileAsync(string path, CancellationToken token);
        Task<byte[][]> LoadLargeBinFileAsync(string path, CancellationToken token);

        IEnumerable<string> GetSearchPaths(string path, string workDir, LoadMode mode)
        {
            if (workDir == null)
                return [];

            if (mode != LoadMode.Json || path.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase))
                return [Path.Combine(workDir, path)];
            return [Path.Combine(workDir, path) + ".json"];
        }
    }
}