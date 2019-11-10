using System;
using System.Collections.Generic;
using AgileVentures.TezPusher.Model.RpcEntities;

namespace AgileVentures.TezPusher.Model.PushEntities
{
    public class HeadModel : Header
    {
        public HeadModel(BlockRpcEntity model)
        {
            protocol = model.protocol;
            chain_id = model.chain_id;
            hash = model.hash;
            level = model.header.level;
            proto = model.header.proto;
            predecessor = model.header.predecessor;
            timestamp = model.header.timestamp;
            validation_pass = model.header.validation_pass;
            operations_hash = model.header.operations_hash;
            fitness = model.header.fitness;
            context = model.header.context;
            proof_of_work_nonce = model.header.proof_of_work_nonce;
            signature = model.header.signature;
        }

        public string protocol { get; set; }
        public string chain_id { get; set; }
        public string hash { get; set; }
    }
}