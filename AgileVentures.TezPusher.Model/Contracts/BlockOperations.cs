using System.Collections.Generic;
using AgileVentures.TezPusher.Model.RpcEntities;

namespace AgileVentures.TezPusher.Model.Contracts
{
    public class BlockOperations
    {
        public List<BlockOperation<BlockTransactionContent>> Transactions { get; set; }
        public List<BlockOperation<BlockOriginationContent>> Originations { get; set; }
        public List<BlockOperation<BlockDelegationContent>> Delegations { get; set; }
    }
}