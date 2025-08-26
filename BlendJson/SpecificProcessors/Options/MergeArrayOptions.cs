using System.Collections.Generic;
using BlendJson.DataSources;
using Newtonsoft.Json;

namespace BlendJson.SpecificProcessors.Options
{
    public class MergeArrayOptions : IStringOption
    {
        public IDataSource DataSource { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableProcessors { get; set; }
        public IDictionary<string, object> Parameters { get; set; }

        public void FillFromString(string str)
        {
            DataSource = new FileDataSource() { Path = str };
        }
    }
}
