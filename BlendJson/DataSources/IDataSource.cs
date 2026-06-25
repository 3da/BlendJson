using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace BlendJson.DataSources
{
    public interface IDataSource
    {
        Task<(JsonNode, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context, CancellationToken token = default);

        string? WorkDir => null;
        Encoding? Encoding => null;
    }
}