using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.SpecificProcessors
{
    internal class UnionArrayProcessor : ISpecificProcessor
    {
        public string KeyWord => "union-";
        public bool IsPrefix => true;
        public async Task<JsonNode> DoAsync(ParseContext context, JsonNode jOptions, JsonObject obj, string keyWord,
            CancellationToken token = default)
        {
            await Task.Yield();
            var propName = keyWord.Substring(KeyWord.Length);

            var otherArr = obj[propName] as JsonArray;


            if (jOptions is not JsonArray arr)
                throw new SettingsException()
                {
                    JToken = jOptions
                };

            if (otherArr == null)
            {
                obj[propName] = jOptions;
            }
            else
            {
                foreach (var item in arr)
                {
                    if (!otherArr.Any(item2 => JsonNode.DeepEquals(item2, item)))
                        otherArr.Add(item?.DeepClone());
                }
            }

            return obj;
        }
    }
}
