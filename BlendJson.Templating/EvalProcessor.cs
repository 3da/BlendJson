using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.SpecificProcessors;
using BlendJson.SpecificProcessors.Options;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

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
        public async Task<JsonNode> DoAsync(ParseContext context, JsonNode jOptions, JsonObject obj, string keyWord,
            CancellationToken token = default)
        {
            var options = Common.ParseOptions<EvalOptions>(jOptions, context.SerializerOptions).Single();

            try
            {

                var globals = GlobalsProcessor.Process(_globals, context);

                var result = await CSharpScript.EvaluateAsync(options.Expression, globals: globals,
                    options: ScriptOptions.Default.WithReferences("Microsoft.CSharp"), cancellationToken: token);

                return result != null ? JsonSerializer.SerializeToNode(result) : null;
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
