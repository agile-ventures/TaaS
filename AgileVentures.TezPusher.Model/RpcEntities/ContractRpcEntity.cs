using AgileVentures.TezPusher.Model.Interfaces;

namespace AgileVentures.TezPusher.Model.RpcEntities
{
    public class Delegate
    {
        public bool setable { get; set; }
    }

    public class ContractRpcEntity : IRpcEntity
    {
        public string manager { get; set; }
        public string balance { get; set; }
        public bool spendable { get; set; }
        public Delegate @delegate { get; set; }
        public int counter { get; set; }
    }

    public class ContractBalanceRpcEntity : IRpcEntity
    {
        public string balance { get; set; }
    }
}