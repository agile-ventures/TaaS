using System;
using System.Collections.Generic;
using AgileVentures.TezPusher.Model.RpcEntities;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Model.PushEntities
{
    public class PushMessage
    {
        public PushMessage(TransactionModel transaction)
        {
            Transaction = transaction;
        }

        public PushMessage(HeadModel blockHeader)
        {
            this.BlockHeader = blockHeader;
        }

        [JsonProperty(PropertyName = "block_header", NullValueHandling = NullValueHandling.Ignore)]
        public HeadModel BlockHeader { get; set; }

        [JsonProperty(PropertyName = "transaction", NullValueHandling = NullValueHandling.Ignore)]
        public TransactionModel Transaction { get; set; }
    }
}