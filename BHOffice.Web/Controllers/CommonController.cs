using BHOffice.Core.Business;
using BHOffice.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BHOffice.Web.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IUserManager _UserManager;

        public CommonController(IUserManager userManager)
        {
            _UserManager = userManager;
        }

        [ChildActionOnly]
        public ActionResult LeftMenu()
        {
            var model = new Models.Common.LeftMenuModel();
            if (this.CurrentUser != null)
            {
                model.IsAdmin = _UserManager.GetUser(this.CurrentUser.Uid).Role >= UserRoles.Admin;
            }
            return View(model);
        }

    }
}
