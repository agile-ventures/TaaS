using System.Collections.Generic;
using AgileVentures.TezPusher.Model.Interfaces;

namespace AgileVentures.TezPusher.Model.RpcEntities
{
    public class ErrorRpcEntity : IRpcEntity
    {
        public string kind { get; set; }
        public string id { get; set; }
        public List<string> missing_key { get; set; }
        public string function { get; set; }
    }
}
