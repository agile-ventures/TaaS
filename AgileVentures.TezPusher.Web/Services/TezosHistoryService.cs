using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.PushEntities;
using AgileVentures.TezPusher.Model.RpcEntities;
using AgileVentures.TezPusher.Web.Configurations;
using AgileVentures.TezPusher.Web.HttpClients;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Web.Services
{
    public interface ITezosHistoryService
    {
        Task ProcessHistoryAsync(IClientProxy clientsCaller, string contextConnectionId, SubscribeModel model);
    }

    public class TezosHistoryService : ITezosHistoryService
    {
        private readonly ILogger<TezosHistoryService> _logger;
        private readonly HttpClient _tezosMonitorClient;
        private readonly TezosConfig _tezosConfig;
        private readonly IPushHistoryService _pushHistoryService;

        private const string TezosBlockUriTemplate = "{0}/chains/main/blocks/{1}";

        public TezosHistoryService(ILogger<TezosHistoryService> logger, TezosMonitorClient tezosMonitorClient, IOptions<TezosConfig> tezosConfig, IPushHistoryService pushHistoryService)
        {
            _logger = logger;
            _tezosMonitorClient = tezosMonitorClient.Client;
            _tezosConfig = tezosConfig.Value;
            _pushHistoryService = pushHistoryService;
        }

        public async Task ProcessHistoryAsync(IClientProxy clientsCaller, string contextConnectionId, SubscribeModel model)
        {
            _logger.LogInformation($"Start processing history from block {model.FromBlockLevel} for connectionId {contextConnectionId}.");
            HttpResponseMessage response;
            Debug.Assert(model.FromBlockLevel != null, "model.FromBlockLevel != null");
            var blockLevel = model.FromBlockLevel.Value;
            do
            {
                response = await _tezosMonitorClient.GetAsync(string.Format(TezosBlockUriTemplate, _tezosConfig.NodeUrl, blockLevel));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var block = JsonConvert.DeserializeObject<BlockRpcEntity>(await response.Content.ReadAsStringAsync());
                    _logger.LogInformation($"Processing history | Block={block.metadata.level}.");

                    await _pushHistoryService.PushBlockHeader(clientsCaller, new HeadModel(block));
                    await _pushHistoryService.PushOperations(clientsCaller, block, model);
                }

                blockLevel++;
            } while (response.StatusCode != HttpStatusCode.NotFound);
            _logger.LogInformation($"Finished processing history from block {model.FromBlockLevel} for connectionId {contextConnectionId}.");
        }
    }
}