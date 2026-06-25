using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.SpecificProcessors
{
    internal class ExceptArrayProcessor : ISpecificProcessor
    {
        public string KeyWord => "except-";
        public bool IsPrefix => true;
        public async Task<JsonNode> DoAsync(ParseContext context, JsonNode jOptions, JsonObject obj, string keyWord,
            CancellationToken token = default)
        {
            await Task.Yield();
            var arr = jOptions as JsonArray;

            if (arr == null)
                throw new SettingsException("Cannot except array");

            var propName = keyWord.Substring(KeyWord.Length);

            var otherArr = obj[propName] as JsonArray;


            if (otherArr == null)
                return obj;

            foreach (var item in arr)
            {
                var item2 = otherArr.FirstOrDefault(q => JsonNode.DeepEquals(q, item));
                if (item2 != null)
                    otherArr.Remove(item2);
            }

            return obj;
        }
    }
}
