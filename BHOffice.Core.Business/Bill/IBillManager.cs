using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBillManager
    {
        IBill GetBill(long bid);
        IQueryable<Data.Bill> Search(IUser user, IBillSearchQuery query);
        IQueryable<Data.Bill> GetBill(string[] nos);
        IQueryable<Data.Bill> GetBill(long[] ids);
        IQueryable<Data.BillStateHistory> GetBillHistories(long[] bids);
        IBill GetBill(Data.Bill entity);
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


        public IBill GetBill(long bid)
        {
            ExceptionHelper.ThrowIfNotId(bid, "bid");
            return new BillService(new Lazy<Data.Bill>(() =>
            {
                var entity = _BillRepository.EnableBills.FirstOrDefault(b => b.bid == bid);
                ExceptionHelper.ThrowIfNull(entity, "bid", "运单不存在或已被删除");
                return entity;
            }));
        }

        public IBill GetBill(Data.Bill entity)
        {
            ExceptionHelper.ThrowIfNull(entity, "entity");
            return new BillService(entity);
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
        public IQueryable<Data.Bill> GetBill(long[] ids)
        {
            if (ids == null)
                return Enumerable.Empty<Data.Bill>().AsQueryable();

            ids = ids.Where(id => id > 0).Distinct().ToArray();

            if (ids.Length == 0)
                return Enumerable.Empty<Data.Bill>().AsQueryable();

            return _BillRepository.EnableBills.Where(b => ids.Contains(b.bid));

        }

        public IQueryable<Data.BillStateHistory> GetBillHistories(long[] bids)
        {
            if (bids == null)
                return Enumerable.Empty<Data.BillStateHistory>().AsQueryable();

            bids = bids.Where(id => id > 0).Distinct().ToArray();

            if (bids.Length == 0)
                return Enumerable.Empty<Data.BillStateHistory>().AsQueryable();

            var neetUpdate = _BillRepository
                .Entities
                .Where(b => b.enabled && bids.Contains(b.bid)
                 && !b.finish && (b.next_pull_date == null || b.next_pull_date < DateTime.Now))
                .ToArray()
                .Where(b => !String.IsNullOrWhiteSpace(b.i_express) && !String.IsNullOrWhiteSpace(b.i_no))
                .ToArray();

            if (neetUpdate.Length > 0)
            {
                var updateHistory = new Dictionary<long, Kuaidi100.Kuaidi100Content>();
                var updateHistoryEntities = new Dictionary<long, List<Data.BillStateHistory>>();
                foreach (var item in neetUpdate)
                {
                    try
                    {
                        var result = Kuaidi100.Kuaidi100HistoryProvider.GetHistory(item.i_express, item.i_no);
                        updateHistory.Add(item.bid, result);
                        if (result != null && result.Data != null && result.Data.Length > 0)
                        {
                            updateHistoryEntities.Add(item.bid, result.Data.Select(r => new Data.BillStateHistory
                            {
                                bid = item.bid,
                                c_auto = true,
                                created = DateTime.Now,
                                creater = item.creater,
                                enabled = true,
                                remarks = r.content,
                                state = BillStates.清关完毕国内派送中,
                                state_updated = r.Time
                            }).ToList());
                        }
                    }
                    catch {
                        updateHistory.Add(item.bid, null);
                    }
                }

                

                using (var trans = new System.Transactions.TransactionScope())
                {
                    foreach (var item in neetUpdate)
                    {
                        var result = updateHistory[item.bid];
                        item.finish = result != null && result.IsFinish;
                        item.error = result == null;
                        item.next_pull_date = DateTime.Now.AddHours(3);
                    }
                    _BillRepository.SaveChanges();

                    if (updateHistoryEntities.Count > 0)
                    {
                        long[] delHistoryBids = updateHistoryEntities.Keys.ToArray();
                        _BillStateHistoryRepository.Delete(h => delHistoryBids.Contains(h.bid) && h.c_auto);

                        foreach (var item in updateHistoryEntities)
                        {
                            foreach (var history in item.Value)
                            {
                                _BillStateHistoryRepository.Add(history);
                            }
                        }
                        _BillStateHistoryRepository.SaveChanges();
                    }

                    trans.Complete();
                }
            }

            return _BillStateHistoryRepository
                    .Entities.Where(h => h.enabled && bids.Contains(h.bid));

        }
    }

}
