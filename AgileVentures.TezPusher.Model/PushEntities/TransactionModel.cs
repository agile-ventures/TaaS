using System;
using AgileVentures.TezPusher.Model.RpcEntities;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Model.PushEntities
{
    public class TransactionModel
    {
        public TransactionModel(BlockRpcEntity block, BlockOperation<BlockTransactionContent> transactionOperation, BlockTransactionContent transactionContent)
        {
            BlockHash = block.hash;
            BlockLevel = block.header.level;
            OperationHash = transactionOperation.hash;
            Timestamp = block.header.timestamp;
            TransactionContent = transactionContent;
        }

        [JsonProperty(PropertyName = "block_hash")]
        public string BlockHash { get; set; }

        [JsonProperty(PropertyName = "block_level")]
        public long BlockLevel { get; set; }

        [JsonProperty(PropertyName = "operation_hash")]
        public string OperationHash { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "transaction_content")]
        public BlockTransactionContent TransactionContent { get; set; }
    }
}