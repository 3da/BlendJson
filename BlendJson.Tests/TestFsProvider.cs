using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.Tests
{
    public class TestFsProvider : IFsProvider
    {
        private readonly FsProvider _fsProvider = new FsProvider();

        public List<string> List1 { get; } = new List<string>();

        public bool FileExists(string path)
        {
            return _fsProvider.FileExists(path);
        }

        public async Task<string> LoadTextFileAsync(string path, Encoding encoding, CancellationToken token)
        {
            List1.Add(path);
            return await _fsProvider.LoadTextFileAsync(path, encoding, token);
        }

        public async Task<byte[]> LoadBinFileAsync(string path, CancellationToken token)
        {
            return await _fsProvider.LoadBinFileAsync(path, token);
        }

        public async Task<byte[][]> LoadLargeBinFileAsync(string path, CancellationToken token)
        {
            return await _fsProvider.LoadLargeBinFileAsync(path, token);
        }
    }
}
