using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace AgileVentures.TezPusher.Function
{
    public static class NegotiateFunction
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest req,
            [SignalRConnectionInfo(HubName = "broadcast", UserId = "{headers.x-tezos-live-userid}")] SignalRConnectionInfo connectionInfo,
            Microsoft.Extensions.Logging.ILogger log)
        {
            return connectionInfo;
        }
}
}
