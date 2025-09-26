using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.FsProviders
{
    public class NullFsProvider : IFsProvider
    {
        public static readonly NullFsProvider Instance = new NullFsProvider();

        internal NullFsProvider()
        {
        }

        public Task<bool> FileExistsAsync(string path)
        {
            return Task.FromResult(false);
        }

        public Task<string> LoadTextFileAsync(string path, Encoding encoding, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> LoadBinFileAsync(string path, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<byte[][]> LoadLargeBinFileAsync(string path, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
