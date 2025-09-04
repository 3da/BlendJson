using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BlendJson.DataSources
{
    public interface IDataSource
    {
        Task<(JToken, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context, CancellationToken token = default);

        string WorkDir => null;
        Encoding Encoding => null;
    }
}