using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    internal interface IBillUser
    {
        IBill CreateBill(ContactInfo sender, ContactInfoWithAddress receiver, decimal insurance, string goods, string remarks);
        void DeleteBill(IBill bill);
        void UpdateBill(IBill bill, decimal insurance, string goods, string remarks);
    }
}
