using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBillManager
    {
        IBill Create(IUser user, IBillArgs args);
        IBill GetBill(IUser user, long bid);
    }

    class BillManager : IBillManager
    {
        private readonly Data.IBillRepository _BillRepository;
        private readonly Core.Data.IRepository<Data.BillStateHistory> _BillStateHistoryRepository;

        public BillManager(Data.IBillRepository billRepository,
            Core.Data.IRepository<Data.BillStateHistory> billStateHistoryRepository)
        {
            _BillRepository = billRepository;
            _BillStateHistoryRepository = billStateHistoryRepository;
        }

        public IBill Create(IUser user, IBillArgs args)
        {
            ExceptionHelper.ThrowIfNull(user, "user");

            args.Verify(new AllAllowBillUpdateStrategy(user));

            if(!String.IsNullOrWhiteSpace(args.No))
            {
                var no = args.No.Trim();
                if (_BillRepository.EnableBills.Any(b => b.no == no))
                    throw new BHException(ErrorCode.ArgError, "运单号已存在:" + no);
            }

            var entity = new Data.Bill
            {
                creater = user.Uid,
                created = DateTime.Now,
                last_state_updated = DateTime.Now,
                bill_date = DateTime.Now,
                enabled = true,
                state = BillStates.None,
            };

            args.Fill(new AllAllowBillUpdateStrategy(user), entity, user);

            _BillRepository.Add(entity);
            _BillRepository.SaveChanges();

            return new BillService(user, entity, _BillRepository, _BillStateHistoryRepository);
        }

        //private string CreateBillNo()
        //{
        //    return String.Format("{0:yyyyMMddHHmmssfff}{1}", DateTime.Now, new Random().Next(1, 100000).ToString().PadLeft(5, '0'));
        //}


        public IBill GetBill(IUser user, long bid)
        {
            ExceptionHelper.ThrowIfNotId(bid, "bid");
            return new BillService(user, bid, _BillRepository, _BillStateHistoryRepository);
        }
    }

}
