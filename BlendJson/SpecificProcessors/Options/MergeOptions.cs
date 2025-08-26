using System.Collections.Generic;
using BlendJson.DataSources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlendJson.SpecificProcessors.Options
{
    public class MergeOptions : IStringOption
    {
        public IDataSource DataSource { get; set; }

        public MergePriority Priority { get; set; } = MergePriority.This;

        public MergeNullValueHandling NullValueHandling { get; set; } = MergeNullValueHandling.Merge;
        public bool IgnoreCase { get; set; }

        [JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool? DisableProcessors { get; set; }

        public IDictionary<string, object> Parameters { get; set; }

        public void FillFromString(string str)
        {
            DataSource = new FileDataSource() { Path = str };
        }
    }
}
