using BlendJson.TypeResolving;
using System;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.DataSources
{
    public class StringDataSource(string text) : IDataSource
    {
        public string? WorkDir { get; init; }
        [JsonConverter(typeof(EncodingConverter))]
        public Encoding? Encoding { get; init; }

        public async Task<(JsonNode, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context,
            CancellationToken token)
        {
            await Task.Yield();
            switch (mode)
            {
                case LoadMode.Json:
                    return (JsonNode.Parse(text), this);
                case LoadMode.Text:
                    return (JsonValue.Create(text), this);
                case LoadMode.Bin:
                case LoadMode.LargeBin:
                    throw new InvalidOperationException();
                case LoadMode.Lines:
                    var lines = text.Split('\n').Select(q => q.Trim('\r')).Select(q => JsonValue.Create(q)).ToArray();
                    return (new JsonArray(lines), this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
