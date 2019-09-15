using System.Linq;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.Constants;
using AgileVentures.TezPusher.Model.PushEntities;
using AgileVentures.TezPusher.Model.RpcEntities;
using AgileVentures.TezPusher.Pusher.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AgileVentures.TezPusher.Pusher.Web.Services
{
    public interface IPushService
    {
        Task PushBlockHeader(HeadModel model);
        Task PushTransactions(BlockRpcEntity blockModel);
    }

    public class PushService : IPushService
    {
        private readonly IHubContext<TezosHub> _hubContext;
        private readonly ILogger<PushService> _log;

        public PushService(IHubContext<TezosHub> hubContext, ILogger<PushService> log)
        {
            _hubContext = hubContext;
            _log = log;
        }

        public async Task PushBlockHeader(HeadModel model)
        {
            await _hubContext.Clients.All.SendAsync("block_headers", new PushMessage(model));
            //await _hubContext.Clients.Groups("block_headers").SendAsync("block_headers", new PushMessage(model));
            _log.LogInformation($"Block {model.level} processed. Block header messages have been sent.");
        }

        public async Task PushTransactions(BlockRpcEntity model)
        {
            var transactions = model.GetTransactions();
            foreach (var transaction in transactions)
            {
                var content = transaction.contents.Where(c => c.kind == TezosBlockOperationConstants.Transaction && c.metadata.operation_result.status == TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                foreach (var transactionContent in content)
                {
                    var message = new PushMessage(new TransactionModel(model, transaction, transactionContent));
                    await _hubContext.Clients.Groups(
                            $"transactions_{transactionContent.source}",
                            $"transactions_{transactionContent.destination}",
                            "transactions_all"
                        ).
                        SendAsync("transactions", message);

                    _log.LogTrace($"Block {model.header.level} | " +
                                  $"Operation hash {transaction.hash} has been sent to the following groups [transactions_{transactionContent.source}, transactions_{transactionContent.destination}, transactions_all]");
                }
            }
            _log.LogInformation($"Block {model.header.level} processed. Transaction messages have been sent.");
        }
    }
}