using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BlendJson.SpecificProcessors
{
    internal class ExceptArrayProcessor : ISpecificProcessor
    {
        public string KeyWord => "except-";
        public bool IsPrefix => true;
        public async Task<JToken> DoAsync(ParseContext context, JToken jOptions, JObject obj, string keyWord,
            CancellationToken token = default)
        {
            await Task.Yield();
            var arr = jOptions as JArray;

            if (arr == null)
                throw new SettingsException("Cannot except array");

            var propName = keyWord.Substring(KeyWord.Length);

            var otherArr = obj[propName] as JArray;


            if (otherArr == null)
                return obj;

            foreach (var item in arr)
            {
                var item2 = otherArr.Children().FirstOrDefault(q => JToken.DeepEquals(q, item));
                if (item2 != null)
                    otherArr.Remove(item2);
            }

            return obj;
        }
    }
}
