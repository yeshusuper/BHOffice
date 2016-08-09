using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Bill
{
    public class WayModel
    {
        public class WayItemModel
        {
            public string No { get; set; }
            public BHOffice.Core.Business.Bill.BillStates State { get; set; }
            public string StateName { get { return BHOffice.Core.Business.Bill.BillStates.None == State ? "未更新状态" : State.ToString(); } }
            public string InternalExpress { get; set; }
            public string InternalNo { get; set; }
            public TrackModel.HistoryItem[] Histories { get; set; }
        }

        public WayItemModel[] Items { get; set; }
    }
}