using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.SpecificProcessors.Options;

namespace BlendJson.SpecificProcessors
{
    internal class LoadProcessor : ISpecificProcessor
    {
        public string KeyWord { get; } = "LoadFrom";
        public bool IsPrefix => false;

        public async Task<JsonNode?> DoAsync(ParseContext context, JsonNode jOptions, JsonObject obj, string keyWord,
            CancellationToken token = default)
        {
            var options = Common.ParseOptions<LoadOptions>(jOptions, context.SerializerOptions).Single();

            context = context.Clone();

            context.DisableProcessors = options.DisableProcessors ?? false;

            if (context.Parameters == null)
            {
                context.Parameters = options.Parameters;
            }
            else if (options.Parameters != null)
            {
                foreach (KeyValuePair<string, object> optionParameter in options.Parameters)
                {
                    context.Parameters[optionParameter.Key] = optionParameter.Value;
                }
            }

            return await context.Manager.LoadSettingsAsync(options.DataSource, context, options.Mode, token);
        }
    }
}
