using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Web.Core
{
    public class JsonResultEntry
    {
        [JsonProperty(PropertyName = "code")]
        public BHOffice.Core.ErrorCode Code { get; set; }
        [JsonProperty(PropertyName = "msg")]
        public string Message { get; set; }
    }

    public class JsonResultEntry<T>
    {
        [JsonProperty(PropertyName = "code")]
        public BHOffice.Core.ErrorCode Code { get; set; }
        [JsonProperty(PropertyName = "msg")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}
