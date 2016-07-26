﻿using System;
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
        void UpdateState(BillStates state, string remarks, bool addHistoryOnly = false, DateTime? date = null);
        void Delete();
        void DeleteStateHistory(long bhid);
        IQueryable<Data.BillStateHistory> Histories { get; }
    }

    class BillService : IBill
    {
        private readonly IUser _User;
        private readonly long _Bid;
        private readonly Lazy<Data.Bill> _LazyBill;
        private readonly Data.IBillRepository _BillRepository;
        private readonly Core.Data.IRepository<Data.BillStateHistory> _BillStateHistoryRepository;

        #region IBillUpdateStrategy
        private bool IsOwner
        {
            get
            {
                return _User.Role >= UserRoles.Admin
                    || _User.Uid == _LazyBill.Value.creater
                    || (_LazyBill.Value.agent_uid.HasValue && _LazyBill.Value.agent_uid.Value == _User.Uid);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return !IsOwner || (_LazyBill.Value.confirmed && _User.Role < UserRoles.Agent);
            }
        }

        public bool IsSenderAndReceiverReadOnly
        {
            get
            {
                if (!IsOwner)
                    return true;

                return _User.Role < UserRoles.Admin;
            }
        }

        public bool IsAllowUpdateState
        {
            get
            {
                if (!IsOwner)
                    return false;

                return _User.Role >= UserRoles.Agent;
            }
        }

        public bool IsAllowUpdateCreated
        {
            get { return _User.Role >= UserRoles.Agent; }
        }

        public bool IsAllowUpdateAgent
        {
            get { return _User.Role >= UserRoles.Admin; }
        }
        #endregion

        public long Bid { get { return _Bid; } }

        public BillService(IUser user, long bid,
            Data.IBillRepository billRepository,
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
            Data.IBillRepository billRepository,
            Core.Data.IRepository<Data.BillStateHistory> billStateHistoryRepository)
            : this(user, billRepository, billStateHistoryRepository) 
        {
            _Bid = entity.bid;
            _LazyBill = new Lazy<Data.Bill>(() => entity);
        }

        private BillService(IUser user,
            Data.IBillRepository billRepository,
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

            if (!String.IsNullOrWhiteSpace(args.No))
            {
                var no = args.No.Trim();
                if (no != No && _BillRepository.EnableBills.Any(b => b.bid != Bid && b.no == no))
                    throw new BHException(ErrorCode.ArgError, "运单号已存在:" + no);
            }

            args.Fill(this, _LazyBill.Value, _User);

            _BillRepository.SaveChanges();
        }

        public void Delete()
        {
            if(!IsOwner)
                throw new BHException(ErrorCode.NotAllow, "没有删除此运单的权限");
            if(State != BillStates.None)
                throw new BHException(ErrorCode.NotAllow, "不能删除已经发出的运单");

            _LazyBill.Value.enabled = false;
            _BillRepository.SaveChanges();
        }

        public void UpdateState(BillStates state, string remarks, bool addHistoryOnly = false, DateTime? date = null)
        {
            if (!IsAllowUpdateState)
                throw new BHException(ErrorCode.NotAllow, "没有修改此订单的权限");

            if(state == BillStates.None)
                throw new BHException(ErrorCode.NotAllow, "修改的状态不正确");

            if (state == State)
                return;

            var updateBill = false;

            using(var scope = new System.Transactions.TransactionScope())
            {
                if (!_LazyBill.Value.confirmed)
                {
                    _LazyBill.Value.confirmed = true;
                    updateBill = true;
                }
                if (!addHistoryOnly)
                {                   
                    _LazyBill.Value.state = state;
                    _LazyBill.Value.last_state_updated = date ?? DateTime.Now;
                    updateBill = true;
                }
                if (updateBill)
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

            _BillStateHistoryRepository.Update(h => h.bid == Bid && h.bhid == bhid, h => new Data.BillStateHistory
            {
                enabled = false
            });
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
