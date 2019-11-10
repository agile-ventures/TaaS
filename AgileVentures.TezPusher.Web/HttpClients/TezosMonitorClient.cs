using System;
using System.Net.Http;
using System.Net.Http.Headers;
using AgileVentures.TezPusher.Web.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgileVentures.TezPusher.Web.HttpClients
{
    public class TezosMonitorClient
    {
        public HttpClient Client { get; }

        public TezosMonitorClient(HttpClient client, IOptions<TezosConfig> tezosConfig, ILogger<TezosMonitorClient> logger)
        {
            try
            {
                Client = client;
                Client.BaseAddress = new Uri(tezosConfig.Value.NodeUrl);
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            catch (OptionsValidationException ex)
            {
                foreach (var failure in ex.Failures)
                {
                    logger.LogCritical(failure);
                    throw;
                }
            }
        }
    }
}