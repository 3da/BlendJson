using System.Linq;
using System.Text.Json.Nodes;

namespace BlendJson
{
    public static class JsonNodeMerger
    {
        public static JsonNode? Merge(this JsonNode? jsonBase, JsonNode? jsonMerge)
        {
            if (jsonBase == null || jsonMerge == null)
                return jsonBase;

            if (jsonBase is JsonObject jsonBaseObj && jsonMerge is JsonObject jsonMergeObj)
            {
                var props = jsonMergeObj.ToArray();
                jsonMergeObj.Clear();

                foreach (var (key, newValue) in props)
                {
                    switch (jsonBaseObj[key])
                    {
                        case JsonObject jsonBaseChildObj when newValue is JsonObject jsonMergeChildObj:
                            jsonBaseObj[key] = jsonBaseChildObj.Merge(jsonMergeChildObj);
                            break;
                        case JsonArray jsonBaseChildArray when newValue is JsonArray jsonMergeChildArray:
                            jsonBaseObj[key] = jsonBaseChildArray.Merge(jsonMergeChildArray);
                            break;
                        default:
                        {
                            jsonBaseObj[key] = newValue;

                            break;
                        }
                    }
                }
            }
            else
            {
                return jsonMerge;
            }

            return jsonBase;
        }
    }
}
