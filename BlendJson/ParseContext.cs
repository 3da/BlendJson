using System;
using System.Collections.Generic;
using BlendJson.DataSources;
using Newtonsoft.Json;

namespace BlendJson
{
    public class ParseContext : ICloneable
    {
        public IDataSource DataSource { get; set; }
        public bool MergeArray { get; set; }
        public SettingsManager Manager { get; set; }
        public JsonSerializer Serializer { get; set; }
        public bool DisableProcessors { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public IFsProvider FsProvider { get; set; }

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
                FsProvider = FsProvider
            };
        }

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }
}
