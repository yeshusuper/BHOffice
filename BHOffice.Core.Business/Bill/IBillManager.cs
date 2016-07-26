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
        IQueryable<Data.Bill> Search(IUser user, IBillSearchQuery query);
        IQueryable<Data.Bill> GetBill(string[] nos);
        IQueryable<Data.BillStateHistory> GetBillHistories(long[] bids);

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

            if (user.Role == UserRoles.User)
                throw new BHException(ErrorCode.NotAllow, "普通用户暂不支持创建运单");

            var strategy = new AllAllowBillUpdateStrategy(user);

            args.Verify(strategy);

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

            args.Fill(strategy, entity, user);

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


        public IQueryable<Data.Bill> Search(IUser user, IBillSearchQuery query)
        {
            ExceptionHelper.ThrowIfNull(user, "user");
            var source = _BillRepository.EnableBills;
            if (user.Role < UserRoles.Agent)
                source = source.Where(b => b.creater == user.Uid);
            else if (user.Role < UserRoles.Admin)
                source = source.Where(b => b.creater == user.Uid || (b.agent_uid.HasValue && b.agent_uid.Value == user.Uid));

            if (query != null)
            {
                if (!String.IsNullOrWhiteSpace(query.Creater))
                {
                    var creater = query.Creater.Trim();
                    source = source.Where(b => b.Creaters.name == creater || b.Creaters.email == creater);
                }
                if (!String.IsNullOrWhiteSpace(query.No))
                {
                    var no = query.No.Trim();
                    source = source.Where(b => b.no == no);
                }
                if (query.MinCreated.HasValue)
                {
                    source = source.Where(b => b.created >= query.MinCreated.Value);
                }
                if (query.MaxCreated.HasValue)
                {
                    source = source.Where(b => b.created <= query.MaxCreated.Value);
                }
                if (!String.IsNullOrWhiteSpace(query.Receiver))
                {
                    var receiver = query.Receiver.Trim();
                    source = source.Where(b => b.receiver == receiver);
                }
                if (query.State.HasValue)
                {
                    source = source.Where(b => b.state == query.State.Value);
                }
            }
            return source;
        }


        public IQueryable<Data.Bill> GetBill(string[] nos)
        {
            if (nos == null)
                return Enumerable.Empty<Data.Bill>().AsQueryable();

            nos = nos.Where(no => !String.IsNullOrWhiteSpace(no)).Select(no => no.Trim()).Distinct().ToArray();

            if (nos.Length == 0)
                return Enumerable.Empty<Data.Bill>().AsQueryable();

            return _BillRepository.EnableBills.Where(b => nos.Contains(b.no));

        }

        public IQueryable<Data.BillStateHistory> GetBillHistories(long[] bids)
        {
            if (bids == null)
                return Enumerable.Empty<Data.BillStateHistory>().AsQueryable();

            bids = bids.Where(id => id > 0).Distinct().ToArray();

            if (bids.Length == 0)
                return Enumerable.Empty<Data.BillStateHistory>().AsQueryable();

            return _BillStateHistoryRepository.Entities.Where(h => h.enabled && bids.Contains(h.bid));

        }
    }

}
