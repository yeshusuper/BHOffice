using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Index
{
    public class IndexModel
    {
        public class NoticeItem
        {
            public long Nid { get; set; }
            public string Title { get; set; }
            public DateTime Date { get; set; }
        }

        public NoticeItem[] NoticeItems { get; set; }

        public string[] Nos { get; set; }

        public bool IsLogin { get; set; }
    }
}