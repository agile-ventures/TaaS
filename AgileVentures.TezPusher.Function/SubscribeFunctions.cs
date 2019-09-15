using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.PushEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Function
{
    public static class SubscribeFunctions
    {
        [FunctionName("subscribe")]
        public static async Task<IActionResult> Subscribe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req,
            [SignalR(HubName = "broadcast")]
            IAsyncCollector<SignalRGroupAction> signalRGroupActions,
            ILogger log)
        {
            try
            {
                var requestBody = new StreamReader(req.Body).ReadToEnd();

                if (string.IsNullOrEmpty(requestBody))
                {
                    log.LogError("Payload was null or empty");
                    return new BadRequestResult();
                }

                var model = JsonConvert.DeserializeObject<SubscribeModel>(requestBody);

                var tasks = new List<Task>();
                foreach (var address in model.TransactionAddresses)
                {
                    tasks.Add(signalRGroupActions.AddAsync(
                        new SignalRGroupAction
                        {
                            UserId = model.UserId,
                            GroupName = $"transactions_{address}",
                            Action = GroupAction.Add
                        }));
                }

                await Task.WhenAll(tasks.ToArray());
                return new OkResult();

            }
            catch (Exception e)
            {
                log.LogError(e, "Error during subscribe", req);
                return new BadRequestResult();
            }
        }

        [FunctionName("unsubscribe")]
        public static async Task<IActionResult> Unsubscribe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req,
            [SignalR(HubName = "broadcast")]
            IAsyncCollector<SignalRGroupAction> signalRGroupActions,
            ILogger log)
        {
            try
            {
                var requestBody = new StreamReader(req.Body).ReadToEnd();

                if (string.IsNullOrEmpty(requestBody))
                {
                    log.LogError("Payload was null or empty");
                    return new BadRequestResult();
                }

                var model = JsonConvert.DeserializeObject<SubscribeModel>(requestBody);

                var tasks = new List<Task>();
                foreach (var address in model.TransactionAddresses)
                {
                    tasks.Add(signalRGroupActions.AddAsync(
                        new SignalRGroupAction
                        {
                            UserId = model.UserId,
                            GroupName = $"transactions_{address}",
                            Action = GroupAction.Remove
                        }));
                }

                await Task.WhenAll(tasks.ToArray());
                return new OkResult();

            }
            catch (Exception e)
            {
                log.LogError(e, "Error during unsubscribe", req);
                return new BadRequestResult();
            }
        }
    }
}
