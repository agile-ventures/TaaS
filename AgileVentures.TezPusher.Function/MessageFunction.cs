using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.PushEntities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Function
{
    public static class MessageFunction
    {
        [FunctionName("message")]
        public static async Task BlockHeads(
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

                var blockHeader = JsonConvert.DeserializeObject<HeadModel>(requestBody);

                await signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = "block_headers",
                    Arguments = new object[] { new PushMessage(blockHeader) },
                });
            }
            catch (Exception e)
            {
                log.LogError(e, "Error during message function");
            }
        }
    }
}
