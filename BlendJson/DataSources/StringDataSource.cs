using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BlendJson.DataSources
{
    public class StringDataSource(string text) : IDataSource
    {
        public string WorkDir { get; init; }
        public Encoding Encoding { get; init; }

        public async Task<(JToken, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context,
            CancellationToken token)
        {
            await Task.Yield();
            switch (mode)
            {
                case LoadMode.Json:
                    return (JToken.Parse(text), this);
                case LoadMode.Text:
                    return (new JValue(text), this);
                case LoadMode.Bin:
                case LoadMode.LargeBin:
                    throw new InvalidOperationException();
                case LoadMode.Lines:
                    var lines = text.Split('\n').Select(q => q.Trim('\r')).ToArray();
                    return (new JArray(lines), this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
