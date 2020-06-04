using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Model.PushEntities
{
    public class SubscribeModel
    {
        [JsonProperty(PropertyName = "userId", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "transactionAddresses", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> TransactionAddresses { get; set; }

        [JsonProperty(PropertyName = "originationAddresses", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> OriginationAddresses { get; set; }

        [JsonProperty(PropertyName = "delegationAddresses", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> DelegationAddresses { get; set; }

        [JsonProperty(PropertyName = "fromBlockLevel", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? FromBlockLevel { get; set; }

        [JsonProperty(PropertyName = "blockHeaders", NullValueHandling = NullValueHandling.Ignore)]
        public bool? BlockHeaders { get; set; }
    }
}