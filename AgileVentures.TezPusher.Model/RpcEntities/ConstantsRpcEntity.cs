using System.Collections.Generic;
using AgileVentures.TezPusher.Model.Interfaces;

namespace AgileVentures.TezPusher.Model.RpcEntities
{
    public class ConstantsRpcEntity : IRpcEntity
    {
        public int proof_of_work_nonce_size { get; set; }
        public int nonce_length { get; set; }
        public int max_revelations_per_block { get; set; }
        public int preserved_cycles { get; set; }
        public int blocks_per_cycle { get; set; }
        public int blocks_per_commitment { get; set; }
        public int blocks_per_roll_snapshot { get; set; }
        public int blocks_per_voting_period { get; set; }
        public List<string> time_between_blocks { get; set; }
        public int endorsers_per_block { get; set; }
        public string hard_gas_limit_per_operation { get; set; }
        public string hard_gas_limit_per_block { get; set; }
        public string proof_of_work_threshold { get; set; }
        public string dictator_pubkey { get; set; }
        public int max_operation_data_length { get; set; }
        public string tokens_per_roll { get; set; }
        public int michelson_maximum_type_size { get; set; }
        public string seed_nonce_revelation_tip { get; set; }
        public string origination_burn { get; set; }
        public string block_security_deposit { get; set; }
        public string endorsement_security_deposit { get; set; }
        public string block_reward { get; set; }
        public string endorsement_reward { get; set; }
        public string cost_per_byte { get; set; }
        public string hard_storage_limit_per_operation { get; set; }
        public string hard_storage_limit_per_block { get; set; }
    }
}
