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

        [HttpGet]
        public ActionResult Edit(long? bid)
        {
            if(bid.HasValue && bid.Value > 0)
            {
                var user = _UserManager.GetUser(CurrentUser.Uid);
                var bill =_BillManager.GetBill(user, bid.Value);
                return View(new Models.Bill.EditModel(bill));
            }
            else
            {
                return View(new Models.Bill.EditModel());
            }
        }

        [HttpPost]
        public ActionResult Edit(Models.Bill.BillEditModel model)
        {
            try
            {
                var user = _UserManager.GetUser(CurrentUser.Uid);
                IBill bill;
                if (model.Bid > 0)
                {
                    bill = _BillManager.GetBill(user, model.Bid);
                    bill.UpdateInfo(model);
                }
                else
                {
                    bill = _BillManager.Create(user, model);
                }
                return View(new Models.Bill.EditModel(bill));
            }
            catch(System.Exception ex)
            {
                return View(new Models.Bill.EditModel(model)
                {
                    ErrorMessage = ex.Message
                });
            }
        }
    }
}
