using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBill
    {
        long Creater { get; }
        DateTime Created { get; }
        long Bid { get; }
        BillStates State { get; }
        DateTime? LastStateDate { get; }
        long? AgentUid { get; set; }
        BillTradeNo No { get; }
        ContactInfo Sender { get; }
        ContactInfoWithAddress Receiver { get; }
        decimal Insurance { get; }
        string Goods { get; }
        string Remarks { get; }
        InternalTrade InternalTrade { get; }

        void UpdateInfo(decimal insurance, string goods, string remarks);
        void UpdateInfo(ContactInfo sender, ContactInfoWithAddress receiver);
        void Delete();
        void UpdateState(IUser @operator, BillStates state);
        void UpdateInternalState(IUser @operator, InternalTrade trade);
        void UpdateCreated(IUser @operator, DateTime created);
    }

    class BillService : IBill
    {
        private readonly Lazy<Data.Bill> _LazyBill;

        public long Bid { get { return _LazyBill.Value.bid; } }

        public BillService(Lazy<Data.Bill> lazyBill)
        {
            _LazyBill = lazyBill;
        }
        public BillService(Data.Bill entity)
        {
            _LazyBill = new Lazy<Data.Bill>(() => entity);
        }

        public BillStates State
        {
            get { return _LazyBill.Value.state; }
        }

        public DateTime? LastStateDate
        {
            get { return _LazyBill.Value.last_state_updated; }
        }

        public void UpdateInfo(decimal insurance, string goods, string remarks)
        {
            _LazyBill.Value.insurance = Math.Max(0, insurance);
            _LazyBill.Value.goods = goods.SafeTrim() ?? String.Empty;
            _LazyBill.Value.remarks = remarks.SafeTrim() ?? String.Empty;
        }

        public void Delete()
        {
            _LazyBill.Value.enabled = false;
        }

        #region 属性
        public ContactInfo Sender
        {
            get { return new ContactInfo(_LazyBill.Value.sender, _LazyBill.Value.sender_tel); }
        }

        public ContactInfoWithAddress Receiver
        {
            get { return new ContactInfoWithAddress(_LazyBill.Value.receiver, _LazyBill.Value.receiver_tel, new Address(_LazyBill.Value.receiver_addr, _LazyBill.Value.post)); }
        }

        public BillTradeNo No
        {
            get { return _LazyBill.Value.no; }
        }

        public DateTime Created
        {
            get { return _LazyBill.Value.bill_date; }
        }

        public long? AgentUid
        {
            get { return _LazyBill.Value.agent_uid; }
            set { _LazyBill.Value.agent_uid = value; }
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

        public InternalTrade InternalTrade
        {
            get 
            { 
                return new Bill.InternalTrade(_LazyBill.Value.i_no, _LazyBill.Value.i_express); 
            }
        }
        public long Creater
        {
            get { return _LazyBill.Value.creater; }
        }
        #endregion




        private bool Confirme(IUser @operator)
        {
            ExceptionHelper.ThrowIfNull(@operator, "@operator");

            if (!_LazyBill.Value.confirmed)
            {
                _LazyBill.Value.confirmed = true;
                _LazyBill.Value.confirmer = @operator.Uid;
                _LazyBill.Value.updated = DateTime.Now;
                _LazyBill.Value.updater = @operator.Uid;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void UpdateInternalState(IUser @operator, InternalTrade trade)
        {
            ExceptionHelper.ThrowIfNull(trade, "trade");
            _LazyBill.Value.i_no = trade.No;
            _LazyBill.Value.i_express = trade.Express;
            UpdateState(@operator, BillStates.清关完毕国内派送中);
        }

        public void UpdateState(IUser @operator, BillStates state)
        {
            ExceptionHelper.ThrowIfNull(@operator, "@operator");

            if (state == _LazyBill.Value.state)
                return;

            _LazyBill.Value.state = state;
            _LazyBill.Value.last_state_updated = DateTime.Now;
            _LazyBill.Value.updated = DateTime.Now;
            _LazyBill.Value.updater = @operator.Uid;

            Confirme(@operator);
        }


        public void UpdateCreated(IUser @operator, DateTime created)
        {
            ExceptionHelper.ThrowIfNull(@operator, "@operator");
            _LazyBill.Value.bill_date = created;
            _LazyBill.Value.updated = DateTime.Now;
            _LazyBill.Value.updater = @operator.Uid;
        }

        public void UpdateInfo(ContactInfo sender, ContactInfoWithAddress receiver)
        {
            ExceptionHelper.ThrowIfNull(sender, "sender");
            ExceptionHelper.ThrowIfNull(receiver, "receiver");

            _LazyBill.Value.sender = sender.Name;
            _LazyBill.Value.sender_tel = sender.Mobile;
            _LazyBill.Value.receiver = receiver.Name;
            _LazyBill.Value.receiver_addr = receiver.Address.Addr;
            _LazyBill.Value.receiver_tel = receiver.Mobile;
            _LazyBill.Value.post = receiver.Address.Post;
        }

        public void InitTradeNo()
        {
            ExceptionHelper.ThrowIfNotId(_LazyBill.Value.bid, "bill.bid", "需要先保存实体");
            _LazyBill.Value.no = new BillTradeNo(_LazyBill.Value.bid);
        }
    }

}
