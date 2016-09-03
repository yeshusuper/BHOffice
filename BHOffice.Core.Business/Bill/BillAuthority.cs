using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public class BillAuthority
    {
        private readonly IUser _User;
        private readonly IBill _Bill;

        public BillAuthority(IUser user, IBill bill)
        {
            ExceptionHelper.ThrowIfNull(user, "user");
            ExceptionHelper.ThrowIfNull(bill, "bill");

            _User = user;
            _Bill = bill;
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
