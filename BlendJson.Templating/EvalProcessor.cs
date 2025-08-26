using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.SpecificProcessors;
using BlendJson.SpecificProcessors.Options;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json.Linq;

namespace BlendJson.Templating
{
    public class EvalProcessor : ISpecificProcessor
    {
        private readonly object _globals;

        public EvalProcessor(object globals)
        {
            _globals = globals;
        }

        public string KeyWord { get; } = "Eval";
        public bool IsPrefix { get; } = false;
        public async Task<JToken> DoAsync(ParseContext context, JToken jOptions, JObject obj, string keyWord,
            CancellationToken token = default)
        {
            var options = Common.ParseOptions<EvalOptions>(jOptions, context.Serializer).Single();

            try
            {

                var globals = GlobalsProcessor.Process(_globals, context);

                var result = await CSharpScript.EvaluateAsync(options.Expression, globals: globals,
                    options: ScriptOptions.Default.WithReferences("Microsoft.CSharp"), cancellationToken: token);

                return result != null ? JToken.FromObject(result) : null;
            }
            catch (Exception e)
            {
                throw new SettingsException($"Error processing: {options.Expression}", e)
                {
                    JToken = jOptions
                };
            }


        }
    }
}
