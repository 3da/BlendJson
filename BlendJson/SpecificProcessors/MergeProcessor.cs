using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.ParseModes;
using BlendJson.SpecificProcessors.Options;

namespace BlendJson.SpecificProcessors
{
    internal class MergeProcessor : ISpecificProcessor
    {
        public string KeyWord { get; } = "MergeWith";
        public bool IsPrefix => false;

        public async Task<JsonNode> DoAsync(ParseContext context, JsonNode jOptions, JsonObject obj, string keyWord,
            CancellationToken token = default)
        {
            var options = Common.ParseOptions<MergeOptions>(jOptions, context.SerializerOptions);

            JsonObject resultObj = obj;

            foreach (var option in options)
            {
                var mergePath = option.DataSource;

                context = context.Clone();

                context.DisableProcessors = option.DisableProcessors ?? false;

                if (context.Parameters == null)
                {
                    context.Parameters = option.Parameters;
                }
                else if (option.Parameters != null)
                {
                    foreach (KeyValuePair<string, object> optionParameter in option.Parameters)
                    {
                        context.Parameters[optionParameter.Key] = optionParameter.Value;
                    }
                }


                var otherToken = await context.Manager.LoadSettingsAsync(mergePath, context, LoadMode.Json, token);

                if (context.MergeMode == null)
                {
                    continue;
                }

                if (otherToken is not JsonObject otherObj)
                    throw new SettingsException($"Merge path {mergePath} must be JObject");


                if (option.Priority == MergePriority.This)
                {
                    otherObj.Merge(resultObj);
                    resultObj = otherObj;
                }
                else if (option.Priority == MergePriority.Other)
                {
                    resultObj.Merge(otherObj);
                }
                else
                    throw new NotImplementedException(option.Priority.ToString());
            }

            return resultObj;
        }
    }
}
