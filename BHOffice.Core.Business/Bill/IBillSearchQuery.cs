using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBillSearchQuery
    {
        string Creater { get; }
        string No { get; }
        string Receiver { get; }
        DateTime? MinCreated { get; }
        DateTime? MaxCreated { get; }
        BillStates? State { get; }
    }
}
