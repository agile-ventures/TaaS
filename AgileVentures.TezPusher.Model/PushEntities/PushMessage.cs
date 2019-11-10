using AgileVentures.TezPusher.Model.Constants;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Model.PushEntities
{
    public class PushMessage
    {
        public PushMessage(DelegationModel delegation)
        {
            Delegation = delegation;
            MessageType = PushMessageTypes.DelegationMessageType;
        }

        public PushMessage(OriginationModel origination)
        {
            Origination = origination;
            MessageType = PushMessageTypes.OriginationMessageType;
        }

        public PushMessage(TransactionModel transaction)
        {
            Transaction = transaction;
            MessageType = PushMessageTypes.TransactionMessageType;
        }

        public PushMessage(HeadModel blockHeader)
        {
            BlockHeader = blockHeader;
            MessageType = PushMessageTypes.BlockHeaderMessageType;
        }

        [JsonProperty(PropertyName = "block_header", NullValueHandling = NullValueHandling.Ignore)]
        public HeadModel BlockHeader { get; set; }

        [JsonProperty(PropertyName = "delegation", NullValueHandling = NullValueHandling.Ignore)]
        public DelegationModel Delegation { get; set; }

        [JsonProperty(PropertyName = "origination", NullValueHandling = NullValueHandling.Ignore)]
        public OriginationModel Origination { get; set; }

        [JsonProperty(PropertyName = "transaction", NullValueHandling = NullValueHandling.Ignore)]
        public TransactionModel Transaction { get; set; }

        [JsonProperty(PropertyName = "message_type")]
        public string MessageType { get; set; }
    }
}