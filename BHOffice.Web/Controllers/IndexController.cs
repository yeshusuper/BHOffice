using BHOffice.Core.Business.Notice;
using BHOffice.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BHOffice.Web.Controllers
{
    public class IndexController : BaseController
    {
        private readonly INoticeManager _NoticeManager;

        public IndexController(INoticeManager noticeManager)
        {
            _NoticeManager = noticeManager;
        }

        public ActionResult Index(string nos)
        {
            var model = new Models.Index.IndexModel
            {
                IsLogin = CurrentUser != null,
                Nos = nos == null ? new String[0] : nos.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries),
                NoticeItems = _NoticeManager.EnabledList
                                .OrderByDescending(n => n.top)
                                .ThenByDescending(n => n.updated)
                                .Select(n => new Models.Index.IndexModel.NoticeItem
                                {
                                    Nid = n.nid,
                                    Title = n.title,
                                    Date = n.updated,
                                })
                                .Take(10)
                                .ToArray()
            };

            return View(model);
        }

    }
}
