using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using BlendJson.DataSources;

namespace BlendJson.ExternalDataSources
{
    public class HttpDataSource : IDataSource
    {
        public string Uri { get; set; }
        public async Task<(JsonNode, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context,
            CancellationToken token)
        {
            if (mode != LoadMode.Json)
                throw new NotImplementedException();

            using var client = new HttpClient();
            var str = await client.GetStringAsync(Uri);

            return (JsonNode.Parse(str), this);
        }
    }
}
