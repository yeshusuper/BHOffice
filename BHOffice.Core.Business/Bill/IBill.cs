using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBill
    {
        string No { get; }
        BillStates State { get; }
        DateTime LastStateDate { get; }
        void UpdateInfo(BillArgs args);
        void UpdateState(BillStates state, string remarks, DateTime? date = null);
    }
}
