using System.Linq;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.Constants;
using AgileVentures.TezPusher.Model.Contracts;
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
        Task PushOperations(BlockRpcEntity blockModel);
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
            _log.LogInformation($"Processing block {model.level}. Block header messages have been sent.");
        }

        public async Task PushOperations(BlockRpcEntity model)
        {
            var operations = model.GetOperations();
            await PushTransactions(model, operations);
            await PushDelegations(model, operations);
            await PushOriginations(model, operations);
            _log.LogInformation($"Processing block {model.header.level}. All operation messages have been sent.");
        }

        private async Task PushTransactions(BlockRpcEntity model, BlockOperations operations)
        {
            foreach (var transaction in operations.Transactions)
            {
                var content = transaction.contents.Where(c =>
                    c.kind == TezosBlockOperationConstants.Transaction && c.metadata.operation_result.status ==
                    TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                foreach (var transactionContent in content)
                {
                    // Babylon upgrade - KT1 transactions are smart contract operations
                    var txSource = transactionContent.GetTransactionSource();
                    var txDestination = transactionContent.GetTransactionDestination();
                    var txContent = transactionContent.GetInternalTransactionContent();

                    var message = new PushMessage(new TransactionModel(model, transaction, txContent));
                    await _hubContext.Clients.Groups(
                            $"transactions_{txSource}",
                            $"transactions_{txDestination}",
                            "transactions_all"
                        )
                        .SendAsync("transactions", message);

                    _log.LogTrace($"Block {model.header.level} | " +
                                  $"Operation hash {transaction.hash} has been sent to the following groups [transactions_{transactionContent.source}, transactions_{transactionContent.destination}, transactions_all]");
                }
            }
        }

        private async Task PushDelegations(BlockRpcEntity model, BlockOperations operations)
        {
            foreach (var delegation in operations.Delegations)
            {
                var content = delegation.contents.Where(c =>
                    c.kind == TezosBlockOperationConstants.Delegation && c.metadata.operation_result.status ==
                    TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                foreach (var delegationContent in content)
                {
                    var message = new PushMessage(new DelegationModel(model, delegation, delegationContent));
                    await _hubContext.Clients.Groups(
                            $"delegations_{delegationContent.source}",
                            $"delegations_{delegationContent.@delegate}",
                            "delegations_all"
                        )
                        .SendAsync("delegations", message);

                    _log.LogTrace($"Block {model.header.level} | " +
                                  $"Operation hash {delegation.hash} has been sent to the following groups [delegations_{delegationContent.source}, delegations_{delegationContent.@delegate}, delegations_all]");
                }
            }
        }

        private async Task PushOriginations(BlockRpcEntity model, BlockOperations operations)
        {
            foreach (var originations in operations.Originations)
            {
                var content = originations.contents.Where(c =>
                    c.kind == TezosBlockOperationConstants.Origination && c.metadata.operation_result.status ==
                    TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                foreach (var originationContent in content)
                {
                    var message = new PushMessage(new OriginationModel(model, originations, originationContent));
                    await _hubContext.Clients.Groups(
                            $"originations_{originationContent.source}",
                            "originations_all"
                        )
                        .SendAsync("originations", message);

                    _log.LogTrace($"Block {model.header.level} | " +
                                  $"Operation hash {originations.hash} has been sent to the following groups [originations_{originationContent.source}, originations_all]");
                }
            }
        }
    }
}