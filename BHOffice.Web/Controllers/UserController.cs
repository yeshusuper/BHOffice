using BHOffice.Core;
using BHOffice.Core.Business;
using BHOffice.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BHOffice.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserManager _UserMangaer;

        public UserController(IUserManager userMangaer)
        {
            _UserMangaer = userMangaer;
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(new Models.User.RegisterModel());
        }

        [HttpPost]
        public ActionResult Register(Models.User.RegisterModel model)
        {
            try
            {
                _UserMangaer.Register(model.UserNo, model.Password, model.UserName);
                return RedirectToAction("index", "index");
            }
            catch (Exception ex)
            {
                model.IsEroor = true;
                model.ErrorMessage = ex.Message;
                return View(model);
            }
        }

        public ActionResult IsUsable(string userNo)
        {
            var result = _UserMangaer.IsUsable(ref userNo);
            if (result)
                return SuccessJsonResult();
            else
                return JsonResult(ErrorCode.Exists, "账号已存在");
        }

        [HttpPost]
        public ActionResult Login(string userNo, string password)
        {
            var user = _UserMangaer.Login(userNo, password);
            CurrentUser = new UserSessionEntry
            {
                Name = user.Name,
                Uid = user.Uid
            };
            if(Request.IsAjaxRequest())
                return SuccessJsonResult();
            else
            {
                var backUrl = Request.GetBackUrl();
                if (!String.IsNullOrWhiteSpace(backUrl))
                    return Redirect(backUrl);
                else
                    return RedirectToAction("index", "index");
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            CurrentUser = null;
            if (Request.IsAjaxRequest())
                return SuccessJsonResult();
            else
                return RedirectToAction("index", "index");
        }
    }
}
