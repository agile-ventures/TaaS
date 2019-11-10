using System;
using AgileVentures.TezPusher.Model.RpcEntities;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Model.PushEntities
{
    public class DelegationModel
    {
        public DelegationModel(BlockRpcEntity block, BlockOperation<BlockDelegationContent> transactionOperation, BlockDelegationContent delegationContent)
        {
            BlockHash = block.hash;
            BlockLevel = block.header.level;
            OperationHash = transactionOperation.hash;
            Timestamp = block.header.timestamp;
            DelegationContent = delegationContent;
        }

        [JsonProperty(PropertyName = "block_hash")]
        public string BlockHash { get; set; }

        [JsonProperty(PropertyName = "block_level")]
        public long BlockLevel { get; set; }

        [JsonProperty(PropertyName = "operation_hash")]
        public string OperationHash { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "delegation_content")]
        public BlockDelegationContent DelegationContent { get; set; }
    }
}