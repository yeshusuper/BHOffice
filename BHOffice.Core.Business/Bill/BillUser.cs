using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHOffice.Core.Data;

namespace BHOffice.Core.Business.Bill
{
    internal class BillUser : IBillUser, IBillManagerUser
    {
        private readonly IUser _User;
        private readonly Data.IBillRepository _BillRepository;
        private readonly Core.Data.IRepository<Data.BillStateHistory> _BillStateHistoryRepository;

        public BillUser(IUser user,
            Data.IBillRepository billRepository, 
            Core.Data.IRepository<Data.BillStateHistory> billStateHistoryRepository)
        {
            _User = user;
            _BillRepository = billRepository;
            _BillStateHistoryRepository = billStateHistoryRepository;
        }

        public IBill CreateBill(ContactInfo sender, ContactInfoWithAddress receiver, decimal insurance, string goods, string remarks)
        {
            ExceptionHelper.ThrowIfNull(sender, "sender");
            ExceptionHelper.ThrowIfNull(receiver, "receiver");

            goods = (goods ?? String.Empty).Trim();
            remarks = (remarks ?? String.Empty).Trim();

            using (var scope = new System.Transactions.TransactionScope())
            {
                var entity = new Data.Bill
                {
                    bill_date = DateTime.Now,
                    confirmed = _User.Role >= UserRoles.Agent,
                    created = DateTime.Now,
                    creater = _User.Uid,
                    enabled = true,
                    last_state_updated = DateTime.Now,
                    updated = DateTime.Now,
                    state = BillStates.None,
                };
                if (entity.confirmed)
                    entity.confirmer = _User.Uid;

                var bill = new BillService(entity);
                bill.UpdateInfo(sender, receiver);
                bill.UpdateInfo(insurance, goods, remarks);

                _BillRepository.Add(entity);
                _BillRepository.SaveChanges();

                bill.InitTradeNo();
                _BillRepository.SaveChanges();

                scope.Complete();

                return bill;
            }
        }

        public void DeleteBill(IBill bill)
        {
            ExceptionHelper.ThrowIfNull(bill, "bill");
            bill.Delete();
            _BillRepository.SaveChanges();
        }

        public void UpdateBillState(IBill bill, BillStates state, string remarks, DateTime? date = null)
        {
            ExceptionHelper.ThrowIfNull(bill, "bill");

            if (state == bill.State)
                return;

            using (var scope = new System.Transactions.TransactionScope())
            {
                bill.UpdateState(_User, state);
                _BillRepository.SaveChanges();

                InsertBillStateHistory(bill, state, remarks, date);

                scope.Complete();
            }
        }

        public void UpdateBillInternalState(IBill bill, InternalTrade trade, string remarks, DateTime? date = null)
        {
            ExceptionHelper.ThrowIfNull(bill, "bill");
            using (var scope = new System.Transactions.TransactionScope())
            {
                var oldState = bill.State;

                bill.UpdateInternalState(_User, trade);
                _BillRepository.SaveChanges();

                if (BillStates.清关完毕国内派送中 != oldState)
                    InsertBillStateHistory(bill, BillStates.清关完毕国内派送中, remarks, date);

                scope.Complete();
            }
        }

        public void InsertBillStateHistory(IBill bill, BillStates state, string remarks, DateTime? date = null)
        {
            ExceptionHelper.ThrowIfNull(bill, "bill");

            var entity = new Data.BillStateHistory
            {
                bid = bill.Bid,
                created = DateTime.Now,
                state_updated = date ?? DateTime.Now,
                creater = _User.Uid,
                remarks = remarks.SafeTrim().GetSafeDbString(),
                state = state,
                enabled = true,
            };

            _BillStateHistoryRepository.Add(entity);
            _BillStateHistoryRepository.SaveChanges();
        }

        public void DeleteBillStateHistory(IBill bill, long bhid)
        {
            ExceptionHelper.ThrowIfNull(bill, "bill");
            ExceptionHelper.ThrowIfNotId(bhid, "bhid");

            _BillStateHistoryRepository.Update(h => h.bid == bill.Bid && h.bhid == bhid, h => new Data.BillStateHistory
            {
                enabled = false
            });
        }

        public void UpdateBill(IBill bill, decimal insurance, string goods, string remarks)
        {
            ExceptionHelper.ThrowIfNull(bill, "bill");
            bill.UpdateInfo(insurance, goods, remarks);
            _BillRepository.SaveChanges();
        }


        public void UpdateBill(IBill bill, ContactInfo sender, ContactInfoWithAddress receiver, decimal insurance, string goods, string remarks)
        {
            ExceptionHelper.ThrowIfNull(bill, "bill");
            bill.UpdateInfo(sender, receiver);
            bill.UpdateInfo(insurance, goods, remarks);
            _BillRepository.SaveChanges();
        }


        public void UpdateAgent(IBill bill, long? agentUid)
        {
            ExceptionHelper.ThrowIfNull(bill, "bill");
            if(agentUid.HasValue)
                ExceptionHelper.ThrowIfNotId(agentUid, "agentUid");
            if(Nullable.Compare(agentUid, bill.AgentUid) != 0)
            {
                bill.AgentUid = agentUid;
                _BillRepository.SaveChanges();
            }
        }


        public void UpdateCreated(IBill bill, DateTime created)
        {
            ExceptionHelper.ThrowIfNull(bill, "bill");
            if (bill.Created != created)
            {
                bill.UpdateCreated(_User, created);
                _BillRepository.SaveChanges();
            }
        }
    }
}
