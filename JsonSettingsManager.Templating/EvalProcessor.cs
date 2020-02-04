﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.Linq;
using System.Text;
using JsonSettingsManager.SpecificProcessors;
using JsonSettingsManager.SpecificProcessors.Options;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.Templating
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
        public JToken Do(ParseContext context, JToken jOptions, JObject obj, string keyWord)
        {
            var options = Common.ParseOptions<EvalOptions>(jOptions, context.Serializer).Single();

            try
            {

                var globals = GlobalsProcessor.Process(_globals, context);

                var result = CSharpScript.EvaluateAsync(options.Expression, globals: globals,
                    options: ScriptOptions.Default.WithReferences("Microsoft.CSharp")).Result;

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
