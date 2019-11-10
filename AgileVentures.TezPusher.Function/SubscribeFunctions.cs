using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;
using System.Web.Http;
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [SignalR(HubName = "broadcast")]
            IAsyncCollector<SignalRGroupAction> signalRGroupActions,
            ILogger log)
        {
            try
            {
                var content = await req.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                {
                    log.LogError("Payload was null or empty");
                    return new BadRequestResult();
                }

                var model = JsonConvert.DeserializeObject<SubscribeModel>(content);

                var tasks = new List<Task>();
                tasks.AddRange(SubscribeAddresses(model.TransactionAddresses, "transactions_", signalRGroupActions, model.UserId));
                tasks.AddRange(SubscribeAddresses(model.OriginationAddresses, "originations_", signalRGroupActions, model.UserId));
                tasks.AddRange(SubscribeAddresses(model.DelegationAddresses, "delegations_", signalRGroupActions, model.UserId));
                
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
                tasks.AddRange(UnsubscribeAddresses(model.TransactionAddresses, "transactions_", signalRGroupActions, model.UserId));
                tasks.AddRange(UnsubscribeAddresses(model.OriginationAddresses, "originations_", signalRGroupActions, model.UserId));
                tasks.AddRange(UnsubscribeAddresses(model.DelegationAddresses, "delegations_", signalRGroupActions, model.UserId));

                await Task.WhenAll(tasks.ToArray());
                return new OkResult();

            }
            catch (Exception e)
            {
                log.LogError(e, "Error during unsubscribe", req);
                return new BadRequestResult();
            }
        }

        private static List<Task> SubscribeAddresses(List<string> model, string prefix,
            IAsyncCollector<SignalRGroupAction> signalRGroupActions, string userId)
        {
            if (model == null) return new List<Task>();

            var tasks = new List<Task>();
            foreach (var address in model)
            {
                tasks.Add(signalRGroupActions.AddAsync(
                    new SignalRGroupAction
                    {
                        UserId = userId,
                        GroupName = $"{prefix}{address}",
                        Action = GroupAction.Add
                    }));
            }

            return tasks;
        }

        private static List<Task> UnsubscribeAddresses(List<string> model, string prefix,
            IAsyncCollector<SignalRGroupAction> signalRGroupActions, string userId)
        {
            if (model == null) return new List<Task>();

            var tasks = new List<Task>();
            foreach (var address in model)
            {
                tasks.Add(signalRGroupActions.AddAsync(
                    new SignalRGroupAction
                    {
                        UserId = userId,
                        GroupName = $"{prefix}{address}",
                        Action = GroupAction.Remove
                    }));
            }

            return tasks;
        }
    }
}
