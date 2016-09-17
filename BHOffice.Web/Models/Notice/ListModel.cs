using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Notice
{
    public class ListModel
    {
        public class Item
        {
            public long Nid { get; set; }
            public string Title { get; set; }
            public DateTime Date { get; set; }
            public bool IsTop { get; set; }
        }

        public BHOffice.Web.Core.PageModel<Item> Items { get; set; }
    }
}