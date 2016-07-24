using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Bill
{
    public class ListModel
    {
        public class SearchModel
        {
            public string Creater { get; set; }
            public string No { get; set; }
            public string Receiver { get; set; }
            [DisplayFormat(NullDisplayText = "", DataFormatString = "yyyy/MM/dd")]
            public DateTime? MinCreated { get; set; }
            [DisplayFormat(NullDisplayText = "", DataFormatString = "yyyy/MM/dd")]
            public DateTime? MaxCreated { get; set; }
            public BHOffice.Core.Business.Bill.BillStates? State { get; set; }
        }

        public class ListItemModel
        {
            public string AgentName { get; set; }
            public string No { get; set; }
            public string CreaterName { get; set; }
            public string ReceiverName { get; set; }
            public string ReceiverAddr { get; set; }
            public string StateName { get; set; }
        }

        public SearchModel Query { get; set; }
    }
}