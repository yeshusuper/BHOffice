using BHOffice.Core.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Bill
{
    public class ListModel
    {
        public class SearchModel : BHOffice.Core.Business.Bill.IBillSearchQuery
        {
            public string Creater { get; set; }
            public string No { get; set; }
            public string Receiver { get; set; }
            [DisplayFormat(NullDisplayText = "", DataFormatString = "{0:yyyy/MM/dd}")]
            public DateTime? MinCreated { get; set; }
            [DisplayFormat(NullDisplayText = "", DataFormatString = "{0:yyyy/MM/dd}")]
            public DateTime? MaxCreated { get; set; }
            public BHOffice.Core.Business.Bill.BillStates? State { get; set; }
            public int PageIndex { get; set; }
        }

        public class ListItemModel
        {
            public long Bid { get; set; }
            public string AgentName { get; set; }
            public string No { get; set; }
            public string CreaterName { get; set; }
            public string ReceiverName { get; set; }
            public string ReceiverAddr { get; set; }
            public string StateName { get; set; }

            [DisplayFormat(DataFormatString = "yyyy/MM/dd HH:mm:ss")]
            public DateTime Created { get; set; }
        }

        public bool IsDisplayTrackButton { get; private set; }
        public bool IsDisplayCreaterQuery { get; private set; }

        public SearchModel Query { get; set; }
        public Core.PageModel<ListItemModel> Items { get; set; }

        public ListModel()
        {
            Query = new SearchModel();
            Items = new Core.PageModel<ListItemModel>(Enumerable.Empty<ListItemModel>(), 1, 0);
        }

        public ListModel(IUser user)
            : this()
        {
            IsDisplayTrackButton = IsDisplayCreaterQuery = user.Role >= UserRoles.Agent;
        }
    }

}