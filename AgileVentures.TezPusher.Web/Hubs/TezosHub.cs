using System;
using System.Threading.Tasks;
using AgileVentures.TezPusher.Model.PushEntities;
using AgileVentures.TezPusher.Web.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace AgileVentures.TezPusher.Web.Hubs
{
    public class TezosHub : Hub
    {
        private ILogger<TezosHub> _log;
        private ITezosHistoryService _tezosHistoryService;

        public TezosHub(ILogger<TezosHub> log, ITezosHistoryService tezosHistoryService)
        {
            _log = log;
            _tezosHistoryService = tezosHistoryService;
        }

        public async Task SendMessage(string method, string data)
        {
            await Clients.All.SendAsync(method, new object[] { data });
        }

        public async Task Subscribe(SubscribeModel model)
        {
            if (model.FromBlockLevel.HasValue)
            {
                await _tezosHistoryService.ProcessHistoryAsync(Clients.Caller, Context.ConnectionId, model);
            }

            if (model.BlockHeaders.HasValue && model.BlockHeaders.Value)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "block_headers");
            }

            if (model.TransactionAddresses != null && model.TransactionAddresses.Any())
            {
                foreach (var address in model.TransactionAddresses)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"transactions_{address}");
                }
            }

            if (model.OriginationAddresses != null && model.OriginationAddresses.Any())
            {
                foreach (var address in model.OriginationAddresses)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"originations_{address}");
                }
            }

            if (model.DelegationAddresses != null && model.DelegationAddresses.Any())
            {
                foreach (var address in model.DelegationAddresses)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"delegations_{address}");
                }
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

            foreach (var address in model.OriginationAddresses)
            {
                try
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"originations_{address}");
                }
                catch (Exception e)
                {
                    _log.LogError(e, $"Unable to unsubscribe ConnectionId={Context.ConnectionId} from originations_{address} group.");
                }
            }

            foreach (var address in model.DelegationAddresses)
            {
                try
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"delegations_{address}");
                }
                catch (Exception e)
                {
                    _log.LogError(e, $"Unable to unsubscribe ConnectionId={Context.ConnectionId} from delegations_{address} group.");
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