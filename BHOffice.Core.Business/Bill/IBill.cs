using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBill : IBillArgs, IBillUpdateStrategy
    {
        long Bid { get; }
        BillStates State { get; }
        DateTime? LastStateDate { get; }
        void UpdateInfo(IBillArgs args);
        void UpdateState(BillStates state, string remarks, DateTime? date = null);
        void DeleteStateHistory(long bhid);
        IQueryable<Data.BillStateHistory> Histories { get; }
    }

    class BillService : IBill
    {
        private readonly IUser _User;
        private readonly long _Bid;
        private readonly Lazy<Data.Bill> _LazyBill;
        private readonly Core.Data.IRepository<Data.Bill> _BillRepository;
        private readonly Core.Data.IRepository<Data.BillStateHistory> _BillStateHistoryRepository;

        #region IBillUpdateStrategy
        public bool IsReadOnly
        {
            get
            {
                return _LazyBill.Value.confirmed && _User.Role < UserRoles.Agent;
            }
        }

        public bool IsSenderAndReceiverReadOnly
        {
            get
            {
                if (!_LazyBill.Value.confirmed)
                    return false;
                else if (_LazyBill.Value.state == BillStates.None)
                    return false;
                else
                    return true;
            }
        }

        public bool IsAllowUpdateState
        {
            get
            {
                return _User.Role >= UserRoles.Agent;
            }
        }
        #endregion

        public long Bid { get { return _Bid; } }

        public BillService(IUser user, long bid,
            Core.Data.IRepository<Data.Bill> billRepository,
            Core.Data.IRepository<Data.BillStateHistory> billStateHistoryRepository)
            : this(user, billRepository, billStateHistoryRepository)
        {
            _Bid = bid;
            _LazyBill = new Lazy<Data.Bill>(() =>
            {
                var entity = billRepository.Entities.FirstOrDefault(b => b.bid == bid);
                ExceptionHelper.ThrowIfNull(entity, "bid", "运单不存在");
                return entity;
            });
        }
        public BillService(IUser user, Data.Bill entity,
            Core.Data.IRepository<Data.Bill> billRepository,
            Core.Data.IRepository<Data.BillStateHistory> billStateHistoryRepository)
            : this(user, billRepository, billStateHistoryRepository) 
        {
            _Bid = entity.bid;
            _LazyBill = new Lazy<Data.Bill>(() => entity);
        }

        private BillService(IUser user, 
            Core.Data.IRepository<Data.Bill> billRepository,
            Core.Data.IRepository<Data.BillStateHistory> billStateHistoryRepository)
        {
            _User = user;
            _BillStateHistoryRepository = billStateHistoryRepository;
            _BillRepository = billRepository;
        }

        public BillStates State
        {
            get { return _LazyBill.Value.state; }
        }

        public DateTime? LastStateDate
        {
            get { return _LazyBill.Value.last_state_updated; }
        }

        public void UpdateInfo(IBillArgs args)
        {
            if (IsReadOnly)
                throw new BHException(ErrorCode.NotAllow, "没有修改此订单的权限");

            args.Verify(this);
            args.Fill(this, _LazyBill.Value, _User);
            _BillRepository.SaveChanges();
        }

        public void UpdateState(BillStates state, string remarks, DateTime? date = null)
        {
            if (IsAllowUpdateState)
                throw new BHException(ErrorCode.NotAllow, "没有修改此订单的权限");

            using(var scope = new System.Transactions.TransactionScope())
            {
                _LazyBill.Value.state = state;
                _LazyBill.Value.last_state_updated = date ?? DateTime.Now;
                _BillRepository.SaveChanges();

                var entity = new Data.BillStateHistory
                {
                    bid = Bid,
                    created = DateTime.Now,
                    state_updated = date ?? DateTime.Now,
                    creater = _User.Uid,
                    remarks = remarks.SafeTrim(),
                    state = state,
                    enabled = true,
                };
                _BillStateHistoryRepository.Add(entity);
                _BillStateHistoryRepository.SaveChanges();

                scope.Complete();
            }

        }

        public IQueryable<Data.BillStateHistory> Histories
        {
            get { return _BillStateHistoryRepository.Entities.Where(h => h.bid == Bid && h.enabled); }
        }

        public void DeleteStateHistory(long bhid)
        {
            ExceptionHelper.ThrowIfNotId(bhid, "bhid");
            var strategy = this as IBillUpdateStrategy;
            if (!strategy.IsAllowUpdateState)
                throw new BHException(ErrorCode.NotAllow, "没有修改此订单的权限");

            _BillStateHistoryRepository.Delete(h => h.bid == Bid && h.bhid == bhid);
        }

        #region IBillArgs
        public string Sender
        {
            get { return _LazyBill.Value.sender; }
        }

        public string SenderTel
        {
            get { return _LazyBill.Value.sender_tel; }
        }

        public string Receiver
        {
            get { return _LazyBill.Value.receiver; }
        }

        public string ReceiverTel
        {
            get { return _LazyBill.Value.receiver_tel; }
        }

        public string ReceiverAddress
        {
            get { return _LazyBill.Value.receiver_addr; }
        }

        public string No
        {
            get { return _LazyBill.Value.no; }
        }

        public DateTime? Created
        {
            get { return _LazyBill.Value.bill_date; }
        }

        public long? AgentUid
        {
            get { return _LazyBill.Value.agent_uid; }
        }

        public string Post
        {
            get { return _LazyBill.Value.post; }
        }

        public decimal Insurance
        {
            get { return _LazyBill.Value.insurance; }
        }

        public string Goods
        {
            get { return _LazyBill.Value.goods; }
        }

        public string Remarks
        {
            get { return _LazyBill.Value.remarks; }
        }

        public string InternalExpress
        {
            get { return _LazyBill.Value.i_express; }
        }

        public string InternalNo
        {
            get { return _LazyBill.Value.i_no; }
        }
        #endregion
    }

}
