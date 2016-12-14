using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Kuaidi100
{
    public class Kuaidi100Result : Kuaidi100Content
    {
        [JsonProperty("status")]
        public Kuaidi100ResultStatus Status { get; set; }
    }
}
