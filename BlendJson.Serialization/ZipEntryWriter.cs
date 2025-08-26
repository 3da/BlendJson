using System;
using System.IO;
using System.IO.Compression;
using BlendJson.DataSources;

namespace BlendJson.Serialization
{
    public class ZipEntryWriter : IWriter
    {
        private readonly ZipArchive _zipArchive;
        private readonly string _basePath;

        public ZipEntryWriter(ZipArchive zipArchive, string basePath)
        {
            _zipArchive = zipArchive;
            _basePath = basePath;
        }


        public void Write(string path, params Stream[] streams)
        {
            var relative = Path.GetRelativePath(_basePath, path);

            var entry = _zipArchive.CreateEntry(relative);

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
