using AgileVentures.TezPusher.Model.Interfaces;

namespace AgileVentures.TezPusher.Model.RpcEntities
{
    public class SelectedRollSnapshotRpcEntity : IRpcEntity
    {
        public int selected_snapshot { get; set; }
    }
}