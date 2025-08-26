using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlendJson
{
    public record class JsonReference(string OriginalRef, string FullPath);
}
