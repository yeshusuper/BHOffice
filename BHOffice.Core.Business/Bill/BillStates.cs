using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public enum BillStates : byte
    {
        None = 0,
        到达分拣中心 = 10,
        航班正常正发往中国 = 20,
        到达口岸等待清关 = 30,
        清关完毕国内派送中 = 40,
        无人签收 = 50,
        已签收 = 60,
    }
}
