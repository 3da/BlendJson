using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BlendJson.SpecificProcessors
{
    internal class ConcatArrayProcessor : ISpecificProcessor
    {
        public string KeyWord => "concat-";
        public bool IsPrefix => true;
        public async Task<JToken> DoAsync(ParseContext context, JToken jOptions, JObject obj, string keyWord,
            CancellationToken token = default)
        {
            await Task.Yield();
            var propName = keyWord.Substring(KeyWord.Length);

            var otherArr = obj[propName] as JArray;

            var arr = jOptions as JArray;

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
                    otherArr.Add(item);
                }
            }

            return obj;
        }
    }
}
