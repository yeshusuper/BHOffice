using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public class BillTradeNo
    {
        private string _InternalTradeNo;
        public BillTradeNo(long billId)
        {
            _InternalTradeNo = String.Format("GD94612{0}", billId.ToString().PadLeft(9, '0'));
        }

        private BillTradeNo(string no)
        {
            _InternalTradeNo = no == null ? null : no.Trim();
        }

        public static implicit operator string(BillTradeNo no)
        {
            return no == null ? null : no._InternalTradeNo;
        }

        public static implicit operator BillTradeNo(string no)
        {
            return no == null ? null : new BillTradeNo(no);
        }
    }
}
