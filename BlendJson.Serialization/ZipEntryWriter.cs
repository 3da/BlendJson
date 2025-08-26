using System;
using System.IO;
using System.IO.Compression;
using BlendJson.DataSources;

namespace BlendJson.Serialization
{
    public class ZipEntryWriter : IWriter
    {
        private readonly ZipArchive _zipArchive;
        private readonly Uri _baseUri;

        public ZipEntryWriter(ZipArchive zipArchive, Uri baseUri)
        {
            _zipArchive = zipArchive;
            _baseUri = baseUri;
        }


        public void Write(string path, params Stream[] streams)
        {
            var uri = new Uri(path);

            var relative = _baseUri.MakeRelativeUri(uri);

            var entry = _zipArchive.CreateEntry(relative.ToString().Replace('/', Path.DirectorySeparatorChar));

            using (var entryStream = entry.Open())
            {
                foreach (var stream in streams)
                {
                    stream.CopyTo(entryStream);
                }
            }
        }

        public IDataSource CreateDataSource(string path)
        {
            return new ZipDataSource() { Path = path };
        }
    }
}
