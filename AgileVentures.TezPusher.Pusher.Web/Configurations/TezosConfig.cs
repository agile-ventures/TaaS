using System.ComponentModel.DataAnnotations;

namespace AgileVentures.TezPusher.Pusher.Web.Configurations
{
    public class TezosConfig
    {
        [Required(ErrorMessage = "Please input a valid URL in ENV settings under Tezos:NodeUrl key.")]
        [Url(ErrorMessage = "Please input a valid URL in ENV settings under Tezos:NodeUrl key.")]
        public string NodeUrl { get; set; }
    }
}