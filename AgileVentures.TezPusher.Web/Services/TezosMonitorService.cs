using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.PushEntities;
using AgileVentures.TezPusher.Model.RpcEntities;
using AgileVentures.TezPusher.Pusher.Web.Configurations;
using AgileVentures.TezPusher.Pusher.Web.HttpClients;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Pusher.Web.Services
{
    public class TezosMonitorService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _tezosMonitorClient;
        private readonly IPushService _pushService;
        private readonly TezosConfig _tezosConfig;
        private const string TezosMonitorUriTemplate = "{0}/monitor/heads/main";

        public TezosMonitorService(
            ILogger<TezosMonitorService> logger, TezosMonitorClient client, IOptions<TezosConfig> tezosConfig, IPushService pushService)
        {
            _logger = logger;
            _pushService = pushService;
            _tezosMonitorClient = client.Client;
            _tezosConfig = tezosConfig.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tezos Monitor Service is starting.");

            var nodeMonitorUrl = string.Format(TezosMonitorUriTemplate, _tezosConfig.NodeUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, nodeMonitorUrl);
            HttpResponseMessage result;
            try
            {
                result = await _tezosMonitorClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"Cannot connect to Tezos Node at {nodeMonitorUrl}");
                throw;
            }
            
            var stream = await result.Content.ReadAsStreamAsync();
            var sr = new StreamReader(stream);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                try
                {
                    var head = JsonConvert.DeserializeObject<MonitorHeadModel>(line);

                    var blockString = await _tezosMonitorClient.GetStringAsync(GetBlockUrl(head.hash));

                    var block = JsonConvert.DeserializeObject<BlockRpcEntity>(blockString);

                    await _pushService.PushBlockHeader(new HeadModel(block));
                    await _pushService.PushOperations(block);

                    _logger.LogInformation($"Block {head.level} has been sent to clients.");
                    _logger.LogTrace(line);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send the block to clients.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Tezos Monitor Service is stopping.");
            return Task.CompletedTask;
        }

        private string GetBlockUrl(string hash)
        {
            return $"{_tezosConfig.NodeUrl}/chains/main/blocks/{hash}";
        }
    }
}