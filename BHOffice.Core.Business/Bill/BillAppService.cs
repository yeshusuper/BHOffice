using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBillAppService
    {
        IBill CreateBill(long uid, string sender, string senderTel, string receiver, string receiverTel, string address, string post, decimal insurance, string goods, string remarks, long? agentUid, DateTime? created, string internalNo, string internalExpress);
        IBill UpdateBill(long uid, long bid, string sender, string senderTel, string receiver, string receiverTel, string address, string post, decimal insurance, string goods, string remarks, long? agentUid, DateTime? created, string internalNo, string internalExpress);
        void UpdateInternalState(long uid, BatchInternalTrade batchInternalTrade);
        void UpdateState(long uid, long[] bids, BillStates state, string remarks, DateTime? date);
        void InsertStateHistory(long uid, long[] bids, BillStates state, string remarks, DateTime? date);
        void DeleteBill(long uid, long bid);
        void DeleteBillStateHistory(long uid, long bid, long bhid);
    }

    internal class BillAppService : IBillAppService
    {
        private readonly IUserManager _UserManager;
        private readonly IBillManager _BillManager;
        private readonly Data.IBillRepository _BillRepository;
        private readonly Core.Data.IRepository<Data.BillStateHistory> _BillStateHistoryRepository;

        public BillAppService(IUserManager userMangaer,
            IBillManager billManager,
            Data.IBillRepository billRepository,
            Core.Data.IRepository<Data.BillStateHistory> billStateHistoryRepository)
        {
            _UserManager = userMangaer;
            _BillManager = billManager;
            _BillRepository = billRepository;
            _BillStateHistoryRepository = billStateHistoryRepository;
        }

        public IBill CreateBill(long uid, string sender, string senderTel, string receiver, string receiverTel, string address, string post, decimal insurance, string goods, string remarks, long? agentUid, DateTime? created, string internalNo, string internalExpress)
        {
            var user = _UserManager.GetUser(uid);
            var creater = new BillUser(user, _BillRepository, _BillStateHistoryRepository);
            using (var scope = new System.Transactions.TransactionScope())
            {
                var bill = creater.CreateBill(new ContactInfo(sender, senderTel),
                    new ContactInfoWithAddress(receiver, receiverTel, new Address(address, post)),
                    insurance, goods, remarks);
                var authority = new BillAuthority(user, bill);
                if (authority.AllowSetAgent || authority.AllowFillAgentSelf)
                {
                    IBillManagerUser manager = creater;
                    manager.UpdateAgent(bill, authority.AllowSetAgent ? agentUid ?? uid : uid);
                }
                if (created.HasValue && authority.AllowSetCreateDate)
                {
                    IBillManagerUser manager = creater;
                    manager.UpdateCreated(bill, created.Value);
                }
                if((!String.IsNullOrWhiteSpace(internalNo) || !String.IsNullOrWhiteSpace(internalExpress)) && authority.AllowUpdateState)
                {
                    IBillManagerUser manager = creater;
                    manager.UpdateBillInternalState(bill, new InternalTrade(internalNo, internalExpress), String.Empty, null);
                }

                scope.Complete();

                return bill;
            }
        }

        public IBill UpdateBill(long uid, long bid, string sender, string senderTel, string receiver, string receiverTel, string address, string post, decimal insurance, string goods, string remarks, long? agentUid, DateTime? created, string internalNo, string internalExpress)
        {
            var user = _UserManager.GetUser(uid);
            var bill = _BillManager.GetBill(bid);
            var authority = new BillAuthority(user, bill);
            if (authority.AllowUpdateSenderOrReceiver || authority.AllowSetAgent || authority.AllowUpdateState || authority.AllowSetCreateDate)
            {
                IBillManagerUser updater = new BillUser(user, _BillRepository, _BillStateHistoryRepository);
                if (authority.AllowUpdateSenderOrReceiver)
                    updater.UpdateBill(bill, new ContactInfo(sender, senderTel), new ContactInfoWithAddress(receiver, receiverTel, new Address(address, post)), insurance, goods, remarks);
                if (authority.AllowSetAgent)
                    updater.UpdateAgent(bill, agentUid);
                if (authority.AllowUpdateState)
                {
                    if (!String.IsNullOrWhiteSpace(internalNo) || !String.IsNullOrWhiteSpace(internalExpress))
                    {
                        updater.UpdateBillInternalState(bill, new InternalTrade(internalNo, internalExpress), String.Empty, null);
                    }
                }
                if(created.HasValue && authority.AllowSetCreateDate)
                {
                    updater.UpdateCreated(bill, created.Value);
                }
            }
            else if (authority.AllowUpdateMinorInfo)
            {
                IBillUser updater = new BillUser(user, _BillRepository, _BillStateHistoryRepository);
                updater.UpdateBill(bill, insurance, goods, remarks);
            }
            return bill;
        }

        public void UpdateInternalState(long uid, BatchInternalTrade batchInternalTrade)
        {
            ExceptionHelper.ThrowIfNull(batchInternalTrade, "batchInternalTrade");

            var user = _UserManager.GetUser(uid);
            IBillManagerUser manager = new BillUser(user, _BillRepository, _BillStateHistoryRepository);

            using (var scope = new System.Transactions.TransactionScope())
            {
                foreach (var item in batchInternalTrade)
                {
                    var bill = _BillManager.GetBill(item.Key);
                    var authority = new BillAuthority(user, bill);
                    if (!authority.AllowUpdateState)
                        throw new BHException(ErrorCode.NotAllow, "没有权限更新运单状态");

                    manager.UpdateBillInternalState(bill, item.Value.InternalTrade, item.Value.Remarks, item.Value.Date);
                }
                scope.Complete();
            }
        }

        public void UpdateState(long uid, long[] bids, BillStates state, string remarks, DateTime? date)
        {
            ExceptionHelper.ThrowIfNullOrEmptyIds(ref bids, "bids");

            var user = _UserManager.GetUser(uid);
            IBillManagerUser manager = new BillUser(user, _BillRepository, _BillStateHistoryRepository);
            
            using (var scope = new System.Transactions.TransactionScope())
            {
                foreach (var bid in bids)
                {
                    var bill = _BillManager.GetBill(bid); 
                    var authority = new BillAuthority(user, bill);
                    if (!authority.AllowUpdateState)
                        throw new BHException(ErrorCode.NotAllow, "没有权限更新运单状态,单号：" + bill.No);
                    manager.UpdateBillState(bill, state, remarks, date);
                }
                scope.Complete();
            }
        }

        public void InsertStateHistory(long uid, long[] bids, BillStates state, string remarks, DateTime? date)
        {
            ExceptionHelper.ThrowIfNullOrEmptyIds(ref bids, "bids");

            var user = _UserManager.GetUser(uid);
            IBillManagerUser manager = new BillUser(user, _BillRepository, _BillStateHistoryRepository);
            using (var scope = new System.Transactions.TransactionScope())
            {
                foreach (var bid in bids)
                {
                    var bill = _BillManager.GetBill(bid);
                    var authority = new BillAuthority(user, bill);
                    if (!authority.AllowUpdateState)
                        throw new BHException(ErrorCode.NotAllow, "没有权限更新运单状态,单号：" + bill.No);
                    manager.InsertBillStateHistory(bill, state, remarks, date);
                }

                scope.Complete();
            }
        }

        public void DeleteBillStateHistory(long uid, long bid, long bhid)
        {
            var user = _UserManager.GetUser(uid);
            var bill = _BillManager.GetBill(bid);
            var authority = new BillAuthority(user, bill);
            if (!authority.AllowUpdateState)
                throw new BHException(ErrorCode.NotAllow, "没有权限更新运单状态");

            IBillManagerUser manager = new BillUser(user, _BillRepository, _BillStateHistoryRepository);
            manager.DeleteBillStateHistory(bill, bhid);
        }

        public void DeleteBill(long uid, long bid)
        {
            var user = _UserManager.GetUser(uid);
            var bill = _BillManager.GetBill(bid);
            var authority = new BillAuthority(user, bill);
            if (!authority.AllowDelete)
                throw new BHException(ErrorCode.NotAllow, "没有权限删除运单");

            IBillManagerUser manager = new BillUser(user, _BillRepository, _BillStateHistoryRepository);
            manager.DeleteBill(bill);
        }
    }
}
