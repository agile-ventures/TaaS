using System;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.PushEntities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AgileVentures.TezPusher.Pusher.Web.Hubs
{
    public class TezosHub : Hub
    {
        private ILogger<TezosHub> _log;

        public TezosHub(ILogger<TezosHub> log)
        {
            _log = log;
        }

        public async Task SendMessage(string method, string data)
        {
            await Clients.All.SendAsync(method, new object[] { data });
        }

        public async Task Subscribe(SubscribeModel model)
        {
            foreach (var address in model.TransactionAddresses)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"transactions_{address}");
            }

            await Clients.Caller.SendAsync("subscribed",new object[] { model });
        }
        public async Task Unsubscribe(SubscribeModel model)
        {
            foreach (var address in model.TransactionAddresses)
            {
                try
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"transactions_{address}");
                }
                catch (Exception e)
                {
                    _log.LogError(e,$"Unable to unsubscribe ConnectionId={Context.ConnectionId} from transactions_{address} group.");
                }
            }

            await Clients.Caller.SendAsync("unsubscribed", new object[] { model });
        }

        public async Task SendTransaction(string sourceAddress, string destinationAddress, PushMessage data)
        {
            await Clients.Groups(
                $"transactions_{sourceAddress}", 
                $"transactions_{destinationAddress}",
                "transactions_all"
                ).
                SendAsync("transactions", new object[] { data });
        }
    }
}