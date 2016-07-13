using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BHOffice.Web.Core
{
    public abstract class BaseController : Controller
    {
        public UserSessionEntry CurrentUser
        {
            get
            {
                return Session.GetCurrentUser();
            }
            set
            {
                Session.SetCurrentUser(value);
            }
        }

        public ActionResult JsonResult(BHOffice.Core.ErrorCode code, string msg)
        {
            return Content(JsonConvert.SerializeObject(new JsonResultEntry
            {
                Code = code,
                Message = msg,
            }));
        }

        public ActionResult JsonResult(BHOffice.Core.BHException ex)
        {
            return JsonResult(ex.ErrorCode, ex.Message);
        }

        public ActionResult SuccessJsonResult()
        {
            return JsonResult(BHOffice.Core.ErrorCode.None, String.Empty);
        }
    }
}
