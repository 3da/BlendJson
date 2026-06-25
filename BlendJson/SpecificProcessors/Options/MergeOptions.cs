using BlendJson.DataSources;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlendJson.SpecificProcessors.Options
{
    public class MergeOptions : IStringOption
    {
        public IDataSource DataSource { get; set; }

        public MergePriority Priority { get; set; } = MergePriority.This;

        public bool IgnoreCase { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DisableProcessors { get; set; }

        public IDictionary<string, object> Parameters { get; set; }

        public void FillFromString(string str)
        {
            DataSource = new FileDataSource() { Path = str };
        }
    }
}
