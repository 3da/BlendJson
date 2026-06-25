using System.Text.Json.Nodes;

namespace BlendJson.Templating
{
    public class ConditionOptions
    {
        public JsonNode Then { get; set; }

        public JsonNode Else { get; set; }

        public string If { get; set; }
    }
}
