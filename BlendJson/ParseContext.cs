using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using BlendJson.DataSources;
using BlendJson.FsProviders;
using BlendJson.ParseModes;

namespace BlendJson
{
    public class ParseContext : ICloneable
    {
        public required IDataSource DataSource { get; init; }
        public bool MergeArray { get; set; }
        public required SettingsLoader Manager { get; init; }
        public JsonSerializerOptions SerializerOptions { get; init; }
        public bool DisableProcessors { get; set; }
        public IDictionary<string, object>? Parameters { get; set; }
        public required IFsProvider FsProvider { get; init; }
        internal GetRefsMode? GetRefsMode { get; init; }
        internal MergeMode? MergeMode { get; init; }
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
                SerializerOptions = SerializerOptions,
                DisableProcessors = DisableProcessors,
                Parameters = Parameters != null ? new Dictionary<string, object>(Parameters) : null,
                FsProvider = FsProvider,
                MergeMode = MergeMode,
                GetRefsMode = GetRefsMode,
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
