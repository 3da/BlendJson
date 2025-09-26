using BlendJson.FsProviders;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.Tests
{
    public class TestFsProvider : FsProvider, IFsProvider
    {
        public List<string> List1 { get; } = new List<string>();

        public Task<bool> FileExistsAsync(string path)
        {
            return base.FileExistsAsync(path);
        }

        public async Task<string> LoadTextFileAsync(string path, Encoding encoding, CancellationToken token)
        {
            List1.Add(path);
            return await base.LoadTextFileAsync(path, encoding, token);
        }

        public async Task<byte[]> LoadBinFileAsync(string path, CancellationToken token)
        {
            return await base.LoadBinFileAsync(path, token);
        }

        public async Task<byte[][]> LoadLargeBinFileAsync(string path, CancellationToken token)
        {
            return await base.LoadLargeBinFileAsync(path, token);
        }
    }
}
