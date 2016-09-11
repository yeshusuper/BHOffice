using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public class InternalTrade
    {
        public string No { get; private set; }
        public string Express { get; private set; }

        public InternalTrade(string no, string express)
        {
            No = no.SafeTrim();
            Express = express.SafeTrim();
        }
    }

    public class BatchInternalTrade : Dictionary<long, BatchInternalTrade.Item>
    {
        public class Item
        {
            public InternalTrade InternalTrade { get; private set; }
            public string Remarks { get; private set; }
            public DateTime? Date { get; private set; }

            public Item(InternalTrade internalTrade, string remarks, DateTime? date)
            {
                this.InternalTrade = internalTrade;
                this.Remarks = remarks;
                this.Date = date;
            }
        }
    }
}
