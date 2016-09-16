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
            catch (BHException ex)
            {
                model.IsEroor = true;
                model.ErrorMessage = ex.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                model.IsEroor = true;
                model.ErrorMessage = ex.Message.ToString();
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
            Session.Login(_UserMangaer, userNo, password);
            if(Request.IsAjaxRequest())
                return SuccessJsonResult(Request.GetBackUrl());
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

        [HttpGet]
        public ActionResult Password()
        {
            return View();
        }

        [HttpPost]
        [BHAuthorize]
        public ActionResult Password(string oldPwd, string newPwd)
        {
            _UserMangaer.ResetPassword(CurrentUser.Uid, oldPwd, newPwd);
            return SuccessJsonResult();
        }

        [HttpGet]
        [BHAuthorize]
        public ActionResult List(Models.User.ListModel.SearchQuery query)
        {
            var size = 30;

            query.Page = Math.Max(1, query.Page);

            var result = _UserMangaer.SearchUser(query);
            var count = result.Count();
            if (query.Page > 1)
                result = result.Skip((query.Page - 1) * size);
            result = result.Take(size);
                
            var model = new Models.User.ListModel
            {
                Query = query,
                Items = new Core.PageModel<Models.User.ListModel.Item>(result.Select(r => new Models.User.ListModel.Item
                {
                    Email = r.email,
                    IsEnabled = r.enabled,
                    Name = r.name,
                    Uid = r.uid,
                    Registered = r.created,
                    Role = r.role
                }), query.Page, (int)Math.Ceiling((double)count / (double)size), count)
            };
            return View(model);
        }

        [HttpPost]
        [BHAuthorize]
        public ActionResult UpdateAgent(long uid, bool state)
        {
            var @operator = _UserMangaer.GetUser(CurrentUser.Uid);
            var target = _UserMangaer.GetUser(uid);
            @operator.UpdateAgent(target, state);

            return SuccessJsonResult();
        }


        [HttpPost]
        [BHAuthorize]
        public ActionResult UpdateLock(long uid, bool state)
        {
            var @operator = _UserMangaer.GetUser(CurrentUser.Uid);
            var target = _UserMangaer.GetUser(uid);
            @operator.UpdateLock(target, state);

            return SuccessJsonResult();
        }
    }
}
