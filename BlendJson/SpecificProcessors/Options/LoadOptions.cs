using System.Collections.Generic;
using BlendJson.DataSources;
using Newtonsoft.Json;

namespace BlendJson.SpecificProcessors.Options
{
    public class LoadOptions : IStringOption
    {
        public IDataSource DataSource { get; set; }

        public string Path
        {
            set
            {
                DataSource = new FileDataSource()
                {
                    Path = value
                };
            }
        }

        public LoadMode Mode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableProcessors { get; set; }

        public IDictionary<string, object> Parameters { get; set; }

        public void FillFromString(string str)
        {
            DataSource = new FileDataSource()
            {
                Path = str
            };
        }
    }
}
