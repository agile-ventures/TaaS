using System.Linq;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.Constants;
using AgileVentures.TezPusher.Model.Contracts;
using AgileVentures.TezPusher.Model.PushEntities;
using AgileVentures.TezPusher.Model.RpcEntities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AgileVentures.TezPusher.Web.Services
{
    public interface IPushHistoryService
    {
        Task PushBlockHeader(IClientProxy clientsCaller, HeadModel model);
        Task PushOperations(IClientProxy clientsCaller, BlockRpcEntity blockModel, SubscribeModel model);
    }

    public class PushHistoryService : IPushHistoryService
    {
        private readonly ILogger<PushHistoryService> _log;

        public PushHistoryService(ILogger<PushHistoryService> log)
        {
            _log = log;
        }

        public async Task PushBlockHeader(IClientProxy clientsCaller, HeadModel model)
        {
            await clientsCaller.SendAsync("block_headers", new PushMessage(model));
            _log.LogDebug($"Processing history block {model.level}. Block header message have been sent.");
        }

        public async Task PushOperations(IClientProxy clientsCaller, BlockRpcEntity blockModel, SubscribeModel subscribeModel)
        {
            var operations = blockModel.GetOperations();
            await PushTransactions(clientsCaller, subscribeModel, blockModel, operations);
            await PushDelegations(clientsCaller, subscribeModel, blockModel, operations);
            await PushOriginations(clientsCaller, subscribeModel, blockModel, operations);
            _log.LogDebug($"Processing history block {blockModel.header.level}. All operation messages have been sent.");
        }

        private async Task PushTransactions(IClientProxy clientsCaller, SubscribeModel subscribeModel,
            BlockRpcEntity model,
            BlockOperations operations)
        {
            if (subscribeModel.TransactionAddresses != null && subscribeModel.TransactionAddresses.Any())
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
                        if (subscribeModel.TransactionAddresses.Contains("all") ||
                            subscribeModel.TransactionAddresses.Contains(txSource) ||
                            subscribeModel.TransactionAddresses.Contains(txDestination))
                        {
                            var message = new PushMessage(new TransactionModel(model, transaction, txContent));
                            await clientsCaller.SendAsync("transactions", message);
                            _log.LogDebug($"History Block {model.header.level} | " +
                                          $"Operation hash {transaction.hash} has been sent. TxSource={txSource}, TxDestination={txDestination}");
                        }
                    }
                }
            }
        }

        private async Task PushDelegations(IClientProxy clientsCaller, SubscribeModel subscribeModel,
            BlockRpcEntity model, BlockOperations operations)
        {
            if (subscribeModel.DelegationAddresses != null && subscribeModel.DelegationAddresses.Any())
            {
                foreach (var delegation in operations.Delegations)
                {
                    var content = delegation.contents.Where(c =>
                        c.kind == TezosBlockOperationConstants.Delegation && c.metadata.operation_result.status ==
                        TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                    foreach (var delegationContent in content)
                    {
                        if (subscribeModel.DelegationAddresses.Contains("all") ||
                            subscribeModel.DelegationAddresses.Contains(delegationContent.source) ||
                            subscribeModel.DelegationAddresses.Contains(delegationContent.@delegate))
                        {
                            var message = new PushMessage(new DelegationModel(model, delegation, delegationContent));
                            await clientsCaller.SendAsync("delegations", message);

                            _log.LogDebug($"History block {model.header.level} | " +
                                          $"Operation hash {delegation.hash} has been sent. DelegationSource={delegationContent.source}, DelegationDestination={delegationContent.@delegate}");
                        }
                    }
                }
            }
        }

        private async Task PushOriginations(IClientProxy clientsCaller, SubscribeModel subscribeModel,
            BlockRpcEntity model,
            BlockOperations operations)
        {
            if (subscribeModel.OriginationAddresses != null && subscribeModel.OriginationAddresses.Any())
            {
                foreach (var originations in operations.Originations)
                {
                    var content = originations.contents.Where(c =>
                        c.kind == TezosBlockOperationConstants.Origination && c.metadata.operation_result.status ==
                        TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                    foreach (var originationContent in content)
                    {
                        if (subscribeModel.OriginationAddresses.Contains("all") ||
                            subscribeModel.OriginationAddresses.Contains(originationContent.source))
                        {
                            var message = new PushMessage(new OriginationModel(model, originations, originationContent));
                            await clientsCaller.SendAsync("originations", message);

                            _log.LogDebug($"History block {model.header.level} | " +
                                          $"Operation hash {originations.hash} has been sent. OriginationSource={originationContent.source}");
                        }
                    }
                }
            }
        }
    }
}