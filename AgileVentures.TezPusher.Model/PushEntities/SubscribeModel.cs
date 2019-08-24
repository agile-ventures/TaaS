using System.Collections.Generic;

namespace AgileVentures.TezPusher.Model.PushEntities
{
    public class SubscribeModel
    {
        public string UserId { get; set; }

        public List<string> TransactionAddresses { get; set; }
    }
}