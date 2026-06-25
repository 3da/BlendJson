using BlendJson.DataSources;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlendJson.SpecificProcessors.Options
{
    public class MergeArrayOptions : IStringOption
    {
        public IDataSource DataSource { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DisableProcessors { get; set; }
        public IDictionary<string, object> Parameters { get; set; }

        public void FillFromString(string str)
        {
            DataSource = new FileDataSource() { Path = str };
        }
    }
}
