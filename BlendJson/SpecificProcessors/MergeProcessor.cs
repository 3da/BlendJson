using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.DataSources;
using BlendJson.ParseModes;
using BlendJson.SpecificProcessors.Options;
using Newtonsoft.Json.Linq;

namespace BlendJson.SpecificProcessors
{
    internal class MergeProcessor : ISpecificProcessor
    {
        public string KeyWord { get; } = "MergeWith";
        public bool IsPrefix => false;

        public async Task<JToken> DoAsync(ParseContext context, JToken jOptions, JObject obj, string keyWord,
            CancellationToken token = default)
        {
            var options = Common.ParseOptions<MergeOptions>(jOptions, context.Serializer);

            JObject resultObj = obj;

            foreach (var option in options)
            {
                var jsonMergeSettings = new JsonMergeSettings
                {
                    MergeNullValueHandling = option.NullValueHandling,
                    MergeArrayHandling = MergeArrayHandling.Replace,
                    PropertyNameComparison =
                        option.IgnoreCase
                            ? StringComparison.OrdinalIgnoreCase
                            : StringComparison.Ordinal
                };

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

                if (context.ParseMode is GetRefsMode)
                {
                    continue;
                }

                var otherObj = otherToken as JObject;

                if (otherObj == null)
                    throw new SettingsException($"Merge path {mergePath} must be JObject");


                if (option.Priority == MergePriority.This)
                {
                    otherObj.Merge(resultObj, jsonMergeSettings);
                    resultObj = otherObj;
                }
                else if (option.Priority == MergePriority.Other)
                {
                    resultObj.Merge(otherObj, jsonMergeSettings);
                }
                else
                    throw new NotImplementedException(option.Priority.ToString());
            }

            return resultObj;
        }
    }
}
