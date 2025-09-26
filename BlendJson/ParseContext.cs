using System;
using System.Collections.Generic;
using System.Text;
using BlendJson.DataSources;
using BlendJson.FsProviders;
using BlendJson.ParseModes;
using Newtonsoft.Json;

namespace BlendJson
{
    public class ParseContext : ICloneable
    {
        public IDataSource DataSource { get; set; }
        public bool MergeArray { get; set; }
        public SettingsLoader Manager { get; set; }
        public JsonSerializer Serializer { get; set; }
        public bool DisableProcessors { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public IFsProvider FsProvider { get; set; }
        internal IParseMode ParseMode { get; set; }
        public int Depth { get; internal set; }
        public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
        public bool CheckFileExist { get; set; }

        public ParseContext Clone()
        {
            return new ParseContext()
            {
                DataSource = DataSource,
                Manager = Manager,
                MergeArray = MergeArray,
                Serializer = Serializer,
                DisableProcessors = DisableProcessors,
                Parameters = Parameters != null ? new Dictionary<string, object>(Parameters) : null,
                FsProvider = FsProvider,
                ParseMode = ParseMode,
                Depth = Depth + 1,
                CheckFileExist = CheckFileExist
            };
        }

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }
}
