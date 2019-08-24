using System.Collections.Generic;
using AgileVentures.TezPusher.Model.Interfaces;

namespace AgileVentures.TezPusher.Model.RpcEntities
{
    public class EndorsingRightsRpcEntity : IRpcEntity
    {
        public List<EndorsingRight> Rights { get; set; }
    }

    public class EndorsingRight
    {
        public long level { get; set; }
        public string @delegate { get; set; }
        public List<int> slots { get; set; }
    }
}
