using BHOffice.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BHOffice.Web.Core
{
    public static class Util
    {
        public static IEnumerable<SelectListItem> GetUserRoleSelectList(BHOffice.Core.Business.UserRoles? state)
        {
            return new[] { 
                new SelectListItem { Text = "用户", Value = Convert.ToInt16(BHOffice.Core.Business.UserRoles.User).ToString(), Selected = state.HasValue && state.Value == BHOffice.Core.Business.UserRoles.User },
                new SelectListItem { Text = "代理商", Value = Convert.ToInt16(BHOffice.Core.Business.UserRoles.Agent).ToString(), Selected = state.HasValue && state.Value == BHOffice.Core.Business.UserRoles.Agent },
                new SelectListItem { Text = "管理员", Value = Convert.ToInt16(BHOffice.Core.Business.UserRoles.Admin).ToString(), Selected = state.HasValue && state.Value == BHOffice.Core.Business.UserRoles.Admin },
            };
        }

        public static IEnumerable<SelectListItem> GetBillStateSelectList(BHOffice.Core.Business.Bill.BillStates? state)
        {
            return new[] { 
                new SelectListItem { Text = BHOffice.Core.Business.Bill.BillStates.到达分拣中心.ToString(), Value = Convert.ToInt16(BHOffice.Core.Business.Bill.BillStates.到达分拣中心).ToString(), Selected = state.HasValue && state.Value == BHOffice.Core.Business.Bill.BillStates.到达分拣中心 },
                new SelectListItem { Text = BHOffice.Core.Business.Bill.BillStates.到达口岸等待清关.ToString(), Value = Convert.ToInt16(BHOffice.Core.Business.Bill.BillStates.到达口岸等待清关).ToString(), Selected = state.HasValue && state.Value == BHOffice.Core.Business.Bill.BillStates.到达口岸等待清关 },
                new SelectListItem { Text = BHOffice.Core.Business.Bill.BillStates.航班正常正发往中国.ToString(), Value = Convert.ToInt16(BHOffice.Core.Business.Bill.BillStates.航班正常正发往中国).ToString(), Selected = state.HasValue && state.Value == BHOffice.Core.Business.Bill.BillStates.航班正常正发往中国 },
                new SelectListItem { Text = BHOffice.Core.Business.Bill.BillStates.清关完毕国内派送中.ToString(), Value = Convert.ToInt16(BHOffice.Core.Business.Bill.BillStates.清关完毕国内派送中).ToString(), Selected = state.HasValue && state.Value == BHOffice.Core.Business.Bill.BillStates.清关完毕国内派送中 },
                new SelectListItem { Text = BHOffice.Core.Business.Bill.BillStates.无人签收.ToString(), Value = Convert.ToInt16(BHOffice.Core.Business.Bill.BillStates.无人签收).ToString(), Selected = state.HasValue && state.Value == BHOffice.Core.Business.Bill.BillStates.无人签收 },
                new SelectListItem { Text = BHOffice.Core.Business.Bill.BillStates.已签收.ToString(), Value = Convert.ToInt16(BHOffice.Core.Business.Bill.BillStates.已签收).ToString(), Selected = state.HasValue && state.Value == BHOffice.Core.Business.Bill.BillStates.已签收 },
            };
        }

    
        public static string UrlCombine(string url, NameValueCollection querys, bool doUrlEncode = true)
        {
            ExceptionHelper.ThrowIfNull(url, "url");
            ExceptionHelper.ThrowIfNull(querys, "querys");

            var oldQuerys = GetUrlQuerys(url, doUrlEncode);
            foreach (var key in querys.AllKeys)
            {
                oldQuerys.Set(key, querys[key]);
            }
            var index = url.IndexOf("?");
            if (index > -1)
                url = url.Left(index + 1);
            else if (oldQuerys.Count > 0)
                url += '?';
            var qList = new List<string>();
            foreach (var key in oldQuerys.AllKeys)
            {
                qList.Add(HttpUtility.UrlEncode(key, Encoding.UTF8) + "=" + (doUrlEncode ? HttpUtility.UrlEncode(oldQuerys[key] ?? String.Empty, Encoding.UTF8) : oldQuerys[key] ?? String.Empty));
            }
            qList.Sort();
            return url + String.Join("&", qList);
        }

        public static NameValueCollection GetUrlQuerys(string url, bool doUrlDecode = true)
        {
            ExceptionHelper.ThrowIfNull(url, "url");
            var index = url.IndexOf("?");
            var query = String.Empty;
            if (index > -1)
            {
                if (url.Length > index + 1)
                    query = url.Substring(index + 1);
            }
            else
            {
                try
                {
                    var uri = new Uri(url);
                }
                catch
                {
                    query = url;
                }
            }
            var nvc = new NameValueCollection();
            if (!String.IsNullOrEmpty(query))
            {
                string[] querys = query.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in querys)
                {
                    int i = item.IndexOf("=");
                    if (i > 0)
                    {
                        if (i < item.Length - 1)
                            nvc[HttpUtility.UrlDecode(item.Left(i), Encoding.UTF8)] = doUrlDecode ? HttpUtility.UrlDecode(item.Right(item.Length - i - 1), Encoding.UTF8) : item.Right(item.Length - i - 1);
                        else
                            nvc[HttpUtility.UrlDecode(item.Left(i), Encoding.UTF8)] = String.Empty;
                    }
                }
            }
            return nvc;
        }
    }
}
