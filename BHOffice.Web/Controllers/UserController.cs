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
    }
}
