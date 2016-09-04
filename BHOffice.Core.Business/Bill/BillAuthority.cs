using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public class BillAuthority
    {
        private interface IBillInfo
        {
            BillStates State { get; }
            long? AgentUid { get; }
            long Creater { get; }
        }

        private class BillWarpper : IBillInfo
        {
            private readonly IBill _Bill;

            public BillStates State
            {
                get { return _Bill.State; }
            }

            public long? AgentUid
            {
                get { return _Bill.AgentUid; }
            }

            public long Creater
            {
                get { return _Bill.Creater; }
            }

            public BillWarpper(IBill bill)
            {
                _Bill = bill;
            }
        }

        private class BillEntityWarpper : IBillInfo
        {
            private readonly Data.Bill _Bill;

            public BillStates State
            {
                get { return _Bill.state; }
            }

            public long? AgentUid
            {
                get { return _Bill.agent_uid; }
            }

            public long Creater
            {
                get { return _Bill.creater; }
            }

            public BillEntityWarpper(Data.Bill bill)
            {
                _Bill = bill;
            }
        }


        private readonly IUser _User;
        private readonly IBillInfo _Bill;

        public BillAuthority(IUser user, IBill bill)
        {
            ExceptionHelper.ThrowIfNull(user, "user");
            ExceptionHelper.ThrowIfNull(bill, "bill");

            _User = user;
            _Bill = new BillWarpper(bill);
        }
        public BillAuthority(IUser user, Data.Bill bill)
        {
            ExceptionHelper.ThrowIfNull(user, "user");
            ExceptionHelper.ThrowIfNull(bill, "bill");

            _User = user;
            _Bill = new BillEntityWarpper(bill);
        }

        public bool AllowDelete
        {
            get
            {
                return _Bill.State == BillStates.None &&
                    (_User.Role >= UserRoles.Admin
                        || (_Bill.AgentUid.HasValue && _User.Role >= UserRoles.Agent && _User.Uid == _Bill.AgentUid.Value)
                        || _User.Uid == _Bill.Creater);
            }
        }

        public bool AllowView
        {
            get
            {
                return _User.Role >= UserRoles.Admin || _User.Uid == _Bill.Creater ||
                            (_Bill.AgentUid.HasValue && _User.Role >= UserRoles.Agent && _User.Uid == _Bill.AgentUid.Value);
            }
        }

        public bool AllowUpdateSenderOrReceiver
        {
            get
            {
                return _User.Role >= UserRoles.Admin && _Bill.State == BillStates.None;
            }
        }

        public bool AllowUpdateMinorInfo
        {
            get
            {
                return (_User.Role >= UserRoles.Admin || _User.Uid == _Bill.Creater || 
                            (_Bill.AgentUid.HasValue && _User.Role >= UserRoles.Agent && _User.Uid == _Bill.AgentUid.Value)) 
                        && _Bill.State == BillStates.None;
            }
        }

        public bool AllowUpdateState
        {
            get
            {
                return _User.Role >= UserRoles.Admin ||
                            (_Bill.AgentUid.HasValue && _User.Role >= UserRoles.Agent && _User.Uid == _Bill.AgentUid.Value);
            }
        }

        public bool AllowSetAgent
        {
            get
            {
                return _User.Role >= UserRoles.Admin;
            }
        }

        public bool AllowFillAgentSelf
        {
            get
            {
                return _User.Role >= UserRoles.Agent;
            }
        }

        public bool AllowSetCreateDate
        {
            get
            {
                return _User.Role >= UserRoles.Agent;
            }
        }
    }
}
