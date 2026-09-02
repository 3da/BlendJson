using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.SpecificProcessors.Options;
using Newtonsoft.Json.Linq;

namespace BlendJson.SpecificProcessors
{
    internal class LoadProcessor : ISpecificProcessor
    {
        public string KeyWord { get; } = "LoadFrom";
        public bool IsPrefix => true;

        public async Task<JToken?> DoAsync(ParseContext context, JToken jOptions, JObject obj, string keyWord,
            CancellationToken token = default)
        {
            var options = Common.ParseOptions<LoadOptions>(jOptions, context.Serializer).Single();

            if (keyWord.Length > KeyWord.Length)
            {
                if (keyWord[KeyWord.Length] != '-')
                    throw new SettingsException("Wrong property key: " + keyWord) { Path = obj.Path };

                var loadModeStr = keyWord[(KeyWord.Length + 1)..];

                if (!Enum.TryParse(loadModeStr, out LoadMode loadMode))
                    throw new SettingsException("Wrong load mode: " + loadModeStr) { Path = obj.Path };

                options.Mode = loadMode;
            }

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
