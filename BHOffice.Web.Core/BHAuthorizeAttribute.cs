﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BHOffice.Web.Core
{
    public class BHAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public BHAuthorizeAttribute()
        {
        }

        protected bool AuthorizeCore(System.Web.HttpContextBase httpContext, UserSessionEntry user)
        {
            return user != null && user.Uid > 0;
            /*
            var manager = DependencyResolver.Current.GetService<BHOffice.Core.Business.IUserManager>();
            httpContext.Session.Login(manager, "", "");*/
        }

        protected ActionResult UnauthorizedResult
        {
            get
            {
                var backUrl = HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl);
                return new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { 
                    { "action", "login" },
                    {"controller", "user"}
                }).SetBackUrl(backUrl);
            }
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return AuthorizeCore(httpContext, httpContext.Session.GetCurrentUser());
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = UnauthorizedResult;
        }
    }
}
