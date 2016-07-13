using BHOffice.Core.Business;
using BHOffice.Core.Business.Bill;
using BHOffice.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BHOffice.Web.Controllers
{
    [BHAuthorize]
    public class BillController : BaseController
    {
        private readonly IBillManager _BillManager;
        private readonly IUserManager _UserManager;

        public BillController(IBillManager billManager,
            IUserManager userManager)
        {
            _BillManager = billManager;
            _UserManager = userManager;
        }

        public ActionResult Edit(long? bid)
        {
            if(bid.HasValue)
            {
                var user = _UserManager.GetUser(CurrentUser.Uid);
                var bill =_BillManager.GetBill(user, bid.Value);
                return View(new Models.Bill.BillEditModel(bill));
            }
            else
            {
                return View(new Models.Bill.BillEditModel());
            }
        }
    }
}
