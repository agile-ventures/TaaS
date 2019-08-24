using System;
using System.Collections.Generic;
using System.Linq;
using AgileVentures.TezPusher.Model.Constants;
using AgileVentures.TezPusher.Model.Interfaces;
using Newtonsoft.Json;

namespace AgileVentures.TezPusher.Model.RpcEntities
{
    public class Header
    {
        public long level { get; set; }
        public int proto { get; set; }
        public string predecessor { get; set; }
        public DateTime timestamp { get; set; }
        public int validation_pass { get; set; }
        public string operations_hash { get; set; }
        public List<string> fitness { get; set; }
        public string context { get; set; }
        public int priority { get; set; }
        public string proof_of_work_nonce { get; set; }
        public string signature { get; set; }
    }

    public class TestChainStatus
    {
        public string status { get; set; }
    }

    public class MaxOperationListLength
    {
        public int max_size { get; set; }
        public int max_op { get; set; }
    }

    public class BlockLevelRpcEntity : IRpcEntity, IBlockLevelRpcEntity
    {
        public long level { get; set; }
        public long level_position { get; set; }
        public long cycle { get; set; }
        public int cycle_position { get; set; }
        public int voting_period { get; set; }
        public int voting_period_position { get; set; }
        public bool expected_commitment { get; set; }
    }

    public class BalanceUpdate
    {
        public string kind { get; set; }
        public string contract { get; set; }
        public string change { get; set; }
        public string category { get; set; }
        public string @delegate { get; set; }
        public int? level { get; set; }
    }

    public class Metadata
    {
        public string protocol { get; set; }
        public string next_protocol { get; set; }
        public TestChainStatus test_chain_status { get; set; }
        public int max_operations_ttl { get; set; }
        public int max_operation_data_length { get; set; }
        public int max_block_header_length { get; set; }
        public List<MaxOperationListLength> max_operation_list_length { get; set; }
        public string baker { get; set; }
        public BlockLevelRpcEntity level { get; set; }
        public string voting_period_kind { get; set; }
        public object nonce_hash { get; set; }
        public string consumed_gas { get; set; }
        public List<object> deactivated { get; set; }
        public List<BalanceUpdate> balance_updates { get; set; }
    }

    public class BlockTransactionBalanceUpdate
    {
        public string kind { get; set; }
        public string contract { get; set; }
        public string change { get; set; }
        public string category { get; set; }
        public string @delegate { get; set; }
        public int? level { get; set; }
    }

    public class BlockTransactionBalanceUpdate2
    {
        public string kind { get; set; }
        public string contract { get; set; }
        public string change { get; set; }
    }

    public class BlockTransactionOperationResult
    {
        public string status { get; set; }
        public List<BlockTransactionBalanceUpdate2> balance_updates { get; set; }
        public string consumed_gas { get; set; }
    }

    public class BlockTransactionMetadata
    {
        public List<BlockTransactionBalanceUpdate> balance_updates { get; set; }
        public BlockTransactionOperationResult operation_result { get; set; }
    }

    public class BlockTransactionContent : IRpcEntity
    {
        public string kind { get; set; }
        public string source { get; set; }
        public string fee { get; set; }
        public string counter { get; set; }
        public string gas_limit { get; set; }
        public string storage_limit { get; set; }
        public string amount { get; set; }
        public string destination { get; set; }
        public BlockTransactionMetadata metadata { get; set; }
    }

    public class BlockEndorsementContent : IRpcEntity
    {
        public string kind { get; set; }
        public int level { get; set; }
        public BlockEndorsementMetadata metadata { get; set; }
    }

    public class BlockOperation<T> where T : IRpcEntity
    {
        public string protocol { get; set; }
        public string chain_id { get; set; }
        public string hash { get; set; }
        public string branch { get; set; }
        public List<T> contents { get; set; }
        public string signature { get; set; }
    }

    public class BlockEndorsementBalanceUpdate
    {
        public string kind { get; set; }
        public string contract { get; set; }
        public string change { get; set; }
        public string category { get; set; }
        public string @delegate { get; set; }
        public int? level { get; set; }
    }

    public class BlockEndorsementMetadata
    {
        public List<BlockEndorsementBalanceUpdate> balance_updates { get; set; }
        public string @delegate { get; set; }
        public List<int> slots { get; set; }
    }

    public class BlockDoubleBakingBlockHash
    {
        public int level { get; set; }
        public int proto { get; set; }
        public string predecessor { get; set; }
        public DateTime timestamp { get; set; }
        public int validation_pass { get; set; }
        public string operations_hash { get; set; }
        public List<string> fitness { get; set; }
        public string context { get; set; }
        public int priority { get; set; }
        public string proof_of_work_nonce { get; set; }
        public string signature { get; set; }
    }

    public class BlockDoubleBakingContent : IRpcEntity
    {
        public string kind { get; set; }
        public BlockDoubleBakingBlockHash bh1 { get; set; }
        public BlockDoubleBakingBlockHash bh2 { get; set; }
        public Metadata metadata { get; set; }
    }

    public class BlockTransactionsRpcEntity : List<BlockOperation<BlockTransactionContent>>, IRpcEntity
    {
    }

    public class BlockEndorsementsRpcEntity : List<BlockOperation<BlockEndorsementContent>>, IRpcEntity
    {
    }

    public class BlockDoubleBakingsRpcEntity : List<BlockOperation<BlockDoubleBakingContent>>, IRpcEntity
    {
    }

    public class BlockOperationsRpcEntity : List<List<object>>, IRpcEntity
    {
    }

    public class BlockRpcEntity : IRpcEntity
    {
        public string protocol { get; set; }
        public string chain_id { get; set; }
        public string hash { get; set; }
        public Header header { get; set; }
        public Metadata metadata { get; set; }
        //TODO strongly type! 
        //array of arrays [endorsements,?,?,transaction||delegation||reveal||double_baking_evidence]
        public BlockOperationsRpcEntity operations { get; set; }

        public List<BlockOperation<BlockTransactionContent>> GetTransactions()
        {
            var transcactionsString = JsonConvert.SerializeObject(operations[3]);
            var transactions = JsonConvert.DeserializeObject<BlockTransactionsRpcEntity>(transcactionsString);
            var appliedTransactions = transactions.Where(t => t.contents.Any(c => c.kind == TezosBlockOperationConstants.Transaction && c.metadata.operation_result.status == TezosBlockOperationConstants.OperationResultStatusApplied)).ToList();
            return appliedTransactions;
        }
    }
}