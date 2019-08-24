using System.Collections.Generic;
using AgileVentures.TezPusher.Model.Interfaces;

namespace AgileVentures.TezPusher.Model.RpcEntities
{
    public class BakingRightsRpcEntity : IRpcEntity
    {
        public List<BakingRight> Rights { get; set; }
    }

    public class BakingRight
    {
        public long level { get; set; }
        public string @delegate { get; set; }
        public int priority { get; set; }
    }
}
