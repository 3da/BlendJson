using System.IO;
using BlendJson.DataSources;

namespace BlendJson.Serialization
{
    public interface IWriter
    {
        void Write(string path, params Stream[] streams);
        IDataSource CreateDataSource(string path);
    }
}