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
}
