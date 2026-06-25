using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.SpecificProcessors
{
    public interface ISpecificProcessor
    {
        string KeyWord { get; }

        bool IsPrefix { get; }

        Task<JsonNode?> DoAsync(ParseContext context, JsonNode jOptions, JsonObject obj, string keyWord, CancellationToken token = default);
    }
}