using BHOffice.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BHOffice.Web.Controllers
{
    public class IndexController : BaseController
    {
        public ActionResult Index()
        {
            var model = new Models.Index.IndexModel
            {
                IsLogin = CurrentUser != null,
            };

            return View(model);
        }

    }
}
