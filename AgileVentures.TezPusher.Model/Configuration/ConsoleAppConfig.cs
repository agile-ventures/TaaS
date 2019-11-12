namespace AgileVentures.TezPusher.Model.Configuration
{
    public class ConsoleAppConfig
    {
        public AzureConfig Azure { get; set; }
        public TezosConfig Tezos { get; set; }
    }

    public class AzureConfig
    {
        public string AzureFunctionUrl { get; set; }
        public string AzureFunctionKey { get; set; }
    }

    public class TezosConfig
    {
        public string NodeUrl { get; set; }
    }
}