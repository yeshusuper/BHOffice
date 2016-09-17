using BHOffice.Core.Business;
using BHOffice.Core.Business.Notice;
using BHOffice.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BHOffice.Web.Controllers
{
    public class NoticeController : BaseController
    {
        private readonly IUserManager _UserMangaer;
        private readonly INoticeManager _NoticeManager;

        public NoticeController(
            IUserManager userMangaer,
            INoticeManager noticeManager)
        {
            _UserMangaer = userMangaer;
            _NoticeManager = noticeManager;
        }

        [HttpGet]
        [BHAuthorize]
        public ActionResult List(int? page)
        {
            int size = 30;
            page = page ?? 1;
            page = Math.Max(1, page.Value);

            var user = _UserMangaer.GetUser(CurrentUser.Uid);
            if (user.Role < UserRoles.Admin)
                return RedirectToAction("index", "index");

            var result = _NoticeManager.EnabledList
                            .OrderByDescending(n => n.top)
                            .ThenByDescending(n => n.nid)
                            .Skip((page.Value - 1) * size)
                            .Take(size)
                            .ToArray();

            var model = new Models.Notice.ListModel
            {
                Items = new PageModel<Models.Notice.ListModel.Item>(result.Select(r => new Models.Notice.ListModel.Item
                {
                    Date = r.updated,
                    IsTop = r.top,
                    Nid = r.nid,
                    Title = r.title
                }), page.Value, (int)Math.Ceiling((double)_NoticeManager.EnabledList.Count() / (double)size))
            };

            return View(model);
        }

        [HttpGet]
        [BHAuthorize]
        public ActionResult Edit(long? id)
        {
            var user = _UserMangaer.GetUser(CurrentUser.Uid);
            if (user.Role < UserRoles.Admin)
                return RedirectToAction("index", "index");

            var model = new Models.Notice.EditModel();
            if (id.HasValue)
            {
                var notice = _NoticeManager.GetNotice(id.Value);
                model.Title = notice.Title;
                model.Content = notice.Content;
            }
            return View(model);
        }

        [HttpPost]
        [BHAuthorize]
        public ActionResult Edit(long? id, Models.Notice.EditModel model)
        {
            var user = _UserMangaer.GetUser(CurrentUser.Uid);
            if (id.HasValue)
            {
                var notice = _NoticeManager.GetNotice(id.Value);
                notice.Update(user, model.Title, model.Content);
            }
            else
            {
                id = _NoticeManager.CreateNotice(user, model.Title, model.Content).Nid;
            }
            return SuccessJsonResult(Url.Action("edit", new { id = id.Value }));
        }

        [HttpPost]
        [BHAuthorize]

        public ActionResult UpdateTop(long id, bool top)
        {
            var user = _UserMangaer.GetUser(CurrentUser.Uid);
            var notice = _NoticeManager.GetNotice(id);
            notice.UpdateTop(user, top);
            return SuccessJsonResult();
        }


        [HttpPost]
        [BHAuthorize]

        public ActionResult Delete(long id)
        {
            var user = _UserMangaer.GetUser(CurrentUser.Uid);
            var notice = _NoticeManager.GetNotice(id);
            notice.UpdateEnabled(user, false);
            return SuccessJsonResult();
        }

        [HttpGet]
        public ActionResult Detail(long id)
        {
            var notice = _NoticeManager.GetNotice(id);
            var model = new Models.Notice.DetailModel
            {
                Content = notice.Content,
                Date = notice.Date,
                Title = notice.Title
            };
            return View(model);
        }
    }
}
