using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BHOffice.Web
{
    public class FilterConfig
    {
        private class BHHandleErrorAttribute : HandleErrorAttribute
        {
            public override void OnException(ExceptionContext filterContext)
            {
                /*
                var bhEx = filterContext.Exception as BHOffice.Core.BHException;
                var errorCode = bhEx == null ? BHOffice.Core.ErrorCode.ServerError : bhEx.ErrorCode;
                var errorMsg = bhEx == null ? filterContext.Exception.ToString() : bhEx.Message;
                if(filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        ContentEncoding = filterContext.HttpContext.Response.ContentEncoding,
                        Data = new Core.JsonResultEntry
                        {
                            Code = errorCode,
                            Message = errorMsg
                        }
                    };
                }
                else
                {
                    var error = new Controllers.ErrorController();
                    error.RouteData.Values["action"] = "index";
                    error.RouteData.Values["controller"] = "error";
                    error.RouteData.DataTokens["code"] = errorCode;
                    error.RouteData.DataTokens["msg"] = errorMsg;
                    filterContext.Result = error.Index();
                }*/
                filterContext.ExceptionHandled = false;
            }
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new BHHandleErrorAttribute());
        }
    }
}