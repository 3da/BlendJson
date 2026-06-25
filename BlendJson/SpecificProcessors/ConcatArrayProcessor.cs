using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.SpecificProcessors
{
    internal class ConcatArrayProcessor : ISpecificProcessor
    {
        public string KeyWord => "concat-";
        public bool IsPrefix => true;
        public async Task<JsonNode> DoAsync(ParseContext context, JsonNode jOptions, JsonObject obj, string keyWord,
            CancellationToken token = default)
        {
            await Task.Yield();
            var propName = keyWord.Substring(KeyWord.Length);

            var otherArr = obj[propName] as JsonArray;

            var arr = jOptions as JsonArray;

            if (arr == null)
                throw new SettingsException("Cannot concat array");

            if (otherArr == null)
            {
                obj[propName] = jOptions;
            }
            else
            {
                foreach (var item in arr)
                {
                    otherArr.Add(item?.DeepClone());
                }
            }

            return obj;
        }
    }
}
