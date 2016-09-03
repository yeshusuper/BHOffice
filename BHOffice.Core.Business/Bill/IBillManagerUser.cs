using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBillManagerUser
    {
        void DeleteBill(IBill bill);
        void UpdateBillState(IBill bill, BillStates state, string remarks, DateTime? date = null);
        /// <summary>
        /// 只插入历史，不改变当前状态
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="state"></param>
        /// <param name="remarks"></param>
        /// <param name="date"></param>
        void InsertBillStateHistory(IBill bill, BillStates state, string remarks, DateTime? date = null);
        void DeleteBillStateHistory(IBill bill, long bhid);
        void UpdateBill(IBill bill, ContactInfo sender, ContactInfoWithAddress receiver, decimal insurance, string goods, string remarks);
        void UpdateBillInternalState(IBill bill, InternalTrade trade, string remarks, DateTime? date = null);
        void UpdateAgent(IBill bill, long? agentUid);
        void UpdateCreated(IBill bill, DateTime created);
    }
}
