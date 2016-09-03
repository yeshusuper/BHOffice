using BHOffice.Core.Business;
using BHOffice.Core.Business.Bill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Bill
{
    public class EditModel 
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public Dictionary<long, string> Agents { get; set; }
        public BillEditModel Bill { get; set; }

        public bool IsSenderAndReceiverReadOnly { get; private set; }
        public bool IsReadOnly { get; private set; }
        public bool IsAllowSetAgent { get; private set; }
        public bool IsAllowUpdateState { get; private set; }
        public bool IsAllowSetCreateDate { get; private set; }

        public EditModel(BillAuthority authority, IBill service)
            : this()
        {
            Bill = new BillEditModel(service);

            IsAllowUpdateState = authority.AllowUpdateState;
            IsReadOnly = !authority.AllowUpdateMinorInfo;
            IsSenderAndReceiverReadOnly = !authority.AllowUpdateSenderOrReceiver;
            IsAllowSetAgent = authority.AllowSetAgent;
            IsAllowSetCreateDate = authority.AllowSetCreateDate;
        }

        /// <summary>
        /// 用于创建运单
        /// </summary>
        /// <param name="user"></param>
        public EditModel(IUser user)
            : this()
        {
            Bill = new BillEditModel();
            IsReadOnly = false;
            IsSenderAndReceiverReadOnly = false;
            IsAllowSetAgent = user.Role >= UserRoles.Admin;
            IsAllowUpdateState = user.Role >= UserRoles.Agent;
            IsAllowSetCreateDate = user.Role >= UserRoles.Agent;
        }

        private EditModel()
        {
            this.Agents = new Dictionary<long, string>();
        }
    }
}