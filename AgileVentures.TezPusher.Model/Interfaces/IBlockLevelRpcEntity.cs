namespace AgileVentures.TezPusher.Model.Interfaces
{
    public interface IBlockLevelRpcEntity
    {
        long cycle { get; set; }
        int cycle_position { get; set; }
        bool expected_commitment { get; set; }
        long level { get; set; }
        long level_position { get; set; }
        int voting_period { get; set; }
        int voting_period_position { get; set; }
    }
}