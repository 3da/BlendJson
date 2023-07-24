﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonSettingsManager.DataSources
{
    public class HttpDataSource : IDataSource
    {
        public string Uri { get; set; }
        public async Task<(JToken, IDataSource)> LoadAsync(IDataSource lastDataSource, LoadMode mode, ParseContext context,
            CancellationToken token)
        {
            if (mode != LoadMode.Json)
                throw new NotImplementedException();

            using var client = new HttpClient();
            var str = await client.GetStringAsync(Uri);

            return (JToken.Parse(str), this);
        }
    }
}
