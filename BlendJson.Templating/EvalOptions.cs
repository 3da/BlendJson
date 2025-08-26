using BlendJson.SpecificProcessors.Options;

namespace BlendJson.Templating
{
    public class EvalOptions : IStringOption
    {
        public string Expression { get; set; }
        public void FillFromString(string str)
        {
            Expression = str;
        }
    }
}
