using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Bill
{
    public class TrackModel
    {
        public class HistoryItem
        {
            public long Bhid { get; set; }
            public DateTime Created { get; set; }
            public BHOffice.Core.Business.Bill.BillStates State { get; set; }
            public string StateName { get { return State.ToString(); } }
            public string Remarks { get; set; }
        }

        public HistoryItem[] Histroys { get; set; }
        public BHOffice.Core.Business.Bill.BillStates State { get; set; }
        public string No { get; set; }
        public long Bid { get; set; }
    }

    public class TrackEditModel
    {
        public long Bid { get; set; }
        public DateTime Created { get; set; }
        public BHOffice.Core.Business.Bill.BillStates State { get; set; }
        public string Remarks { get; set; }
        public bool UpdateState { get; set; }
    }
}