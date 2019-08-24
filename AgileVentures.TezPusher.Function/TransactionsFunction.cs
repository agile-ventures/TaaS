using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model;
using AgileVentures.TezPusher.Model.Constants;
using AgileVentures.TezPusher.Model.PushEntities;
using AgileVentures.TezPusher.Model.RpcEntities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Function
{
    public static class MessageExtendedFunction
    {
        [FunctionName("transactions")]
        public static async Task Run(
                                    [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req,
                                    [SignalR(HubName = "broadcast")]IAsyncCollector<SignalRMessage> signalRMessages,
                                    Microsoft.Extensions.Logging.ILogger log)
        {
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();

                if (string.IsNullOrEmpty(requestBody))
                {
                    log.LogError("Payload was null or empty");
                    return;
                }
                log.LogInformation($"Message with payload {requestBody}");

                var model = JsonConvert.DeserializeObject<BlockRpcEntity>(requestBody);
                var transactions = model.GetTransactions();
                foreach (var transaction in transactions)
                {
                    var content = transaction.contents.Where(c => c.kind == TezosBlockOperationConstants.Transaction && c.metadata.operation_result.status == TezosBlockOperationConstants.OperationResultStatusApplied).ToList();
                    foreach (var transactionContent in content)
                    {
                        await signalRMessages.AddAsync(new SignalRMessage
                        {
                            GroupName = $"transactions_{transactionContent.source}",
                            Arguments = new object[] { new PushMessage(new TransactionModel(model, transaction, transactionContent)) },
                            Target = "transactions"
                        });
                        await signalRMessages.AddAsync(new SignalRMessage
                        {
                            GroupName = $"transactions_{transactionContent.destination}",
                            Arguments = new object[] { new PushMessage(new TransactionModel(model, transaction, transactionContent)) },
                            Target = "transactions"
                        });
                        await signalRMessages.AddAsync(new SignalRMessage
                        {
                            GroupName = "transactions_all",
                            Arguments = new object[] { new PushMessage(new TransactionModel(model, transaction, transactionContent)) },
                            Target = "transactions"
                        });
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e, "Error during running message function");
            }
        }
    }
}
