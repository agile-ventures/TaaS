using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.Constants;
using AgileVentures.TezPusher.Model.Contracts;
using AgileVentures.TezPusher.Model.PushEntities;
using AgileVentures.TezPusher.Model.RpcEntities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Function
{
    public static class MessageFunction
    {
        [FunctionName("message")]
        public static Task Message(
                                    [HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequest req,
                                    [SignalR(HubName = "broadcast")]IAsyncCollector<SignalRMessage> signalRMessages,
                                    ILogger log)
        {
            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                var requestBody = new StreamReader(req.Body).ReadToEnd();

                if (string.IsNullOrEmpty(requestBody))
                {
                    log.LogError("Payload was null or empty");
                    return Task.CompletedTask;
                }
                log.LogTrace($"Message with payload {requestBody}");

                var model = JsonConvert.DeserializeObject<BlockRpcEntity>(requestBody);
                log.LogInformation($"Message with block level {model.header.level}");

                var blockHeader = new HeadModel(model);
                signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = "block_headers",
                    Arguments = new object[] { new PushMessage(blockHeader) }
                });

                var operations = model.GetOperations();
                PushTransactions(signalRMessages, operations, model);
                PushDelegations(signalRMessages, operations, model);
                PushOriginations(signalRMessages, operations, model);
            }
            catch (Exception e)
            {
                log.LogError(e, "Error during running message function");
            }

            return Task.CompletedTask;
        }

        private static void PushOriginations(IAsyncCollector<SignalRMessage> signalRMessages, BlockOperations operations, BlockRpcEntity model)
        {
            foreach (var origination in operations.Originations)
            {
                var content = origination.contents.Where(c =>
                    c.kind == TezosBlockOperationConstants.Origination && c.metadata.operation_result.status ==
                    TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                foreach (var originationContent in content)
                {
                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        GroupName = $"originations_{originationContent.source}",
                        Arguments = new object[] { new PushMessage(new OriginationModel(model, origination, originationContent)) },
                        Target = "originations"
                    });
                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        GroupName = "originations_all",
                        Arguments = new object[] { new PushMessage(new OriginationModel(model, origination, originationContent)) },
                        Target = "originations"
                    });
                }
            }
        }

        private static void PushDelegations(IAsyncCollector<SignalRMessage> signalRMessages, BlockOperations operations, BlockRpcEntity model)
        {
            foreach (var delegation in operations.Delegations)
            {
                var content = delegation.contents.Where(c =>
                    c.kind == TezosBlockOperationConstants.Delegation && c.metadata.operation_result.status ==
                    TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                foreach (var delegationContent in content)
                {
                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        GroupName = $"delegations_{delegationContent.source}",
                        Arguments = new object[] { new PushMessage(new DelegationModel(model, delegation, delegationContent)) },
                        Target = "delegations"
                    });
                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        GroupName = $"delegations_{delegationContent.@delegate}",
                        Arguments = new object[] { new PushMessage(new DelegationModel(model, delegation, delegationContent)) },
                        Target = "delegations"
                    });
                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        GroupName = "delegations_all",
                        Arguments = new object[] { new PushMessage(new DelegationModel(model, delegation, delegationContent)) },
                        Target = "delegations"
                    });
                }
            }
        }

        private static void PushTransactions(IAsyncCollector<SignalRMessage> signalRMessages, BlockOperations operations,
            BlockRpcEntity model)
        {
            foreach (var transaction in operations.Transactions)
            {
                var content = transaction.contents.Where(c =>
                    c.kind == TezosBlockOperationConstants.Transaction && c.metadata.operation_result.status ==
                    TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                foreach (var operationContent in content)
                {
                    var transactionContent = (BlockTransactionContent)operationContent;
                    // Babylon upgrade - KT1 transactions are smart contract operations
                    var txSource = transactionContent.GetTransactionSource();
                    var txDestination = transactionContent.GetTransactionDestination();
                    var txContent = transactionContent.GetInternalTransactionContent();

                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        GroupName = $"transactions_{txSource}",
                        Arguments = new object[] { new PushMessage(new TransactionModel(model, transaction, txContent)) },
                        Target = "transactions"
                    });
                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        GroupName = $"transactions_{txDestination}",
                        Arguments = new object[] { new PushMessage(new TransactionModel(model, transaction, txContent)) },
                        Target = "transactions"
                    });
                    signalRMessages.AddAsync(new SignalRMessage
                    {
                        GroupName = "transactions_all",
                        Arguments = new object[] { new PushMessage(new TransactionModel(model, transaction, txContent)) },
                        Target = "transactions"
                    });
                }
            }
        }
    }
}
