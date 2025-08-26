using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using BlendJson.DataSources;

namespace BlendJson.ParseModes;

internal class GetRefsMode : IParseMode
{
    private readonly ConcurrentDictionary<JsonReference, int> _references = new();
    public JsonReference[] References => _references.OrderBy(q => q.Value).Select(q => q.Key).ToArray();

    private int _counter = 0;

    public void AddRef(JsonReference path)
    {
        _references.TryAdd(path, Interlocked.Increment(ref _counter));
    }


}