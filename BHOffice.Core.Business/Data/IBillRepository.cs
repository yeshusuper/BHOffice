using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Data
{
    public interface IBillRepository : Core.Data.IRepository<Data.Bill>
    {
        IQueryable<Data.Bill> EnableBills { get; }
    }

    internal class BillRepository : DbSetRepository<Data.BHOfficeContext, Data.Bill>, IBillRepository
    {
        public BillRepository(Data.BHOfficeContext context) : base(context) { }

        public IQueryable<Bill> EnableBills
        {
            get { return Entities.Where(b => b.enabled); }
        }
    }
}
