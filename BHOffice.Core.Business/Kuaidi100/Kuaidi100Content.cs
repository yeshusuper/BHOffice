using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Kuaidi100
{
    public class Kuaidi100Content
    {
        public class Item
        {
            [JsonProperty("time")]
            public DateTime Time { get; set; }
            [JsonProperty("context")]
            public string content { get; set; }
        }


        [JsonProperty("state")]
        public Kuaidi100States State { get; set; }
        [JsonProperty("data")]
        public Item[] Data { get; set; }

        [JsonIgnore]
        public bool IsFinish
        {
            get { return State == Kuaidi100States.Sign || State == Kuaidi100States.Deny; }
        }
    }
}
