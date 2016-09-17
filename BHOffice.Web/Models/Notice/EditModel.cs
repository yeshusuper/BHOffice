using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BHOffice.Web.Models.Notice
{
    public class EditModel
    {
        public string Title { get; set; }
        [AllowHtml]
        public string Content { get; set; }
    }
}