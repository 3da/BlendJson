using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace BlendJson.Templating
{
    public static class GlobalsProcessor
    {
        public static object Process(object gl, ParseContext context)
        {
            var globals = gl;

            if (globals != null && !(globals is IDictionary))
                return globals;

            var internalGlobals = new InternalGlobals()
            {
                Fields = new ExpandoObject()
            };

            var expDict = internalGlobals.Fields as IDictionary<string, object>;

            if (globals is IDictionary dict)
            {
                foreach (DictionaryEntry dictionaryEntry in dict)
                {
                    expDict[dictionaryEntry.Key.ToString()] = dictionaryEntry.Value;
                }
            }

            if (context.Parameters != null)
            {
                foreach (var contextParameter in context.Parameters)
                {
                    expDict[contextParameter.Key] = contextParameter.Value;
                }
            }

            globals = internalGlobals;

            return globals;
        }
    }
}
