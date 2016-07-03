using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Data
{
    public class BillStateHistory
    {
        public long bhid { get; set; }
        public long bid { get; set; }
        public long creater { get; set; }
        public DateTime state_updated { get; set; }
        public DateTime created { get; set; }
        public Business.Bill.BillStates state { get; set; }
        public bool enabled { get; set; }
        public string remarks { get; set; }
    }
}
