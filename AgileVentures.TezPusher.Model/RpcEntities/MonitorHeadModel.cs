using System;
using System.Collections.Generic;

namespace AgileVentures.TezPusher.Model.RpcEntities
{
    public class MonitorHeadModel
    {
        public string hash { get; set; }
        public long level { get; set; }
        public long proto { get; set; }
        public string predecessor { get; set; }
        public DateTime timestamp { get; set; }
        public long validation_pass { get; set; }
        public string operations_hash { get; set; }
        public List<string> fitness { get; set; }
        public string context { get; set; }
        public string protocol_data { get; set; }
    }
}