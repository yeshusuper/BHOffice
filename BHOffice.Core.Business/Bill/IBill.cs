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

    class BillService : IBill
    {
        public BillService(long bid) { }
        public BillService(Data.Bill entity) { }

        public string No
        {
            get { throw new NotImplementedException(); }
        }

        public BillStates State
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime LastStateDate
        {
            get { throw new NotImplementedException(); }
        }

        public void UpdateInfo(BillArgs args)
        {
            throw new NotImplementedException();
        }

        public void UpdateState(BillStates state, string remarks, DateTime? date = null)
        {
            throw new NotImplementedException();
        }
    }

}
