using BHOffice.Core.Business;
using BHOffice.Core.Business.Bill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Bill
{
    public class EditModel : IBillUpdateStrategy
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public Dictionary<long, string> Agents { get; set; }
        public BillEditModel Bill { get; set; }

        #region IBillUpdateStrategy
        public bool IsReadOnly { get; set; }

        public bool IsSenderAndReceiverReadOnly { get; set; }
        public bool IsAllowUpdateState { get; set; }

        public bool IsAllowUpdateCreated { get; set; }
        public bool IsAllowUpdateAgent { get; set; }
        #endregion

        public EditModel(IBill service)
        {
            this.Bill = new BillEditModel(service);

            this.IsAllowUpdateState = service.IsAllowUpdateState;
            this.IsReadOnly = service.IsReadOnly;
            this.IsSenderAndReceiverReadOnly = service.IsSenderAndReceiverReadOnly;
            this.IsAllowUpdateCreated = service.IsAllowUpdateCreated;
            this.IsAllowUpdateAgent = service.IsAllowUpdateAgent;
            this.Agents = new Dictionary<long, string>();
        }

        public EditModel(IUser user)
            : this(user, null)
        {
        }

        public EditModel(IUser user, BillEditModel model)
        {
            Bill = model ?? new BillEditModel();
            this.IsReadOnly = false;
            this.IsSenderAndReceiverReadOnly = false;
            this.IsAllowUpdateCreated = user.Role >= UserRoles.Agent;
            this.IsAllowUpdateState = false;
            this.IsAllowUpdateAgent = user.Role >= UserRoles.Admin;
            this.Agents = new Dictionary<long, string>();
        }
    }
}