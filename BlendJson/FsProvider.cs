using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson
{
    public class FsProvider : IFsProvider
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
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
    }
}
