using System;
using System.Collections.Generic;
using System.Linq;

namespace BlendJson.DocumentationLib
{
    internal class Context
    {
        public IList<Type> ProcessedTypes { get; set; }

        public Context Clone() => new Context() { ProcessedTypes = ProcessedTypes.ToList() };
    }
}
