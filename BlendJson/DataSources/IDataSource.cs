using System.Threading;
using System.Threading.Tasks;
using BlendJson.TypeResolving;
using Newtonsoft.Json.Linq;

namespace BlendJson.DataSources
{
    [ResolveType]
    public interface IDataSource
    {
        Task<(JToken, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context, CancellationToken token = default);
    }
}