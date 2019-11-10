using System;
using AgileVentures.TezPusher.Model.RpcEntities;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Model.PushEntities
{
    public class OriginationModel
    {
        public OriginationModel(BlockRpcEntity block, BlockOperation<BlockOriginationContent> transactionOperation, BlockOriginationContent originationContent)
        {
            BlockHash = block.hash;
            BlockLevel = block.header.level;
            OperationHash = transactionOperation.hash;
            Timestamp = block.header.timestamp;
            OriginationContent = originationContent;
        }

        [JsonProperty(PropertyName = "block_hash")]
        public string BlockHash { get; set; }

        [JsonProperty(PropertyName = "block_level")]
        public long BlockLevel { get; set; }

        [JsonProperty(PropertyName = "operation_hash")]
        public string OperationHash { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "origination_content")]
        public BlockOriginationContent OriginationContent { get; set; }
    }
}