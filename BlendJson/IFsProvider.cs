using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson
{
    public interface IFsProvider
    {
        bool FileExists(string path);
        Task<string> LoadTextFileAsync(string path, Encoding encoding, CancellationToken token);
        Task<byte[]> LoadBinFileAsync(string path, CancellationToken token);
        Task<byte[][]> LoadLargeBinFileAsync(string path, CancellationToken token);
    }
}