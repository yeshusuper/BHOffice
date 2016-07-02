using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Data
{
    public class Bill
    {
        public long bid { get; set; }
        public string no { get; set; }
        public string sender { get; set; }
        public string sender_tel { get; set; }
        public string receiver { get; set; }
        public string receiver_tel { get; set; }
        public string receiver_addr { get; set; }
        public string post { get; set; }
        public long? agent_uid { get; set; }
        public string goods { get; set; }
        public decimal insurance { get; set; }
        public string i_express { get; set; }
        public string i_no { get; set; }
        public string remarks { get; set; }
        public DateTime bill_date { get; set; }
        public DateTime created { get; set; }
        public long creater { get; set; }
        public DateTime updated { get; set; }
        public long? updater { get; set; }
    }
}
