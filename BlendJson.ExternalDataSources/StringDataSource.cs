using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.DataSources;
using Newtonsoft.Json.Linq;

namespace BlendJson.ExternalDataSources
{
    public class StringDataSource : IDataSource
    {
        public string Text { get; set; }
        public async Task<(JToken, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context,
            CancellationToken token)
        {
            switch (mode)
            {
                case LoadMode.Json:
                    return (JToken.Parse(Text), this);
                case LoadMode.Text:
                    return (new JValue(Text), this);
                case LoadMode.Bin:
                case LoadMode.LargeBin:
                    throw new InvalidOperationException();
                case LoadMode.Lines:
                    var lines = Text.Split('\n').Select(q => q.Trim('\r')).ToArray();
                    return (new JArray(lines), this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
