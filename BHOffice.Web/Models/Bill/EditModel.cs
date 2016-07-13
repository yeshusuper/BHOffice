﻿using BHOffice.Core.Business.Bill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Bill
{
    public class EditModel : BHOffice.Core.Business.Bill.IBillUpdateStrategy
    {
        public Dictionary<long, string> Agents { get; set; }
        public BillEditModel Bill { get; set; }

        #region IBillUpdateStrategy
        public bool IsReadOnly { get; set; }

        public bool IsSenderAndReceiverReadOnly { get; set; }
        public bool IsAllowUpdateState { get; set; }

        public bool IsAgent { get; set; }
        #endregion

        public EditModel(IBill service)
        {
            this.Bill = new BillEditModel(service);

            this.IsAllowUpdateState = service.IsAllowUpdateState;
            this.IsReadOnly = service.IsReadOnly;
            this.IsSenderAndReceiverReadOnly = service.IsSenderAndReceiverReadOnly;
            this.IsAgent = service.IsAgent;
        }

        public EditModel()
        {
            Bill = new BillEditModel();
        }
    }
}