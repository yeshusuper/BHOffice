﻿using BHOffice.Core.Business;
using BHOffice.Core.Business.Bill;
using BHOffice.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BHOffice.Core.Linq;
using BHOffice.Core;

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
            var user = _UserManager.GetUser(CurrentUser.Uid);
            if(bid.HasValue && bid.Value > 0)
            {
                var bill =_BillManager.GetBill(user, bid.Value);
                return View(new Models.Bill.EditModel(bill));
            }
            else
            {
                return View(new Models.Bill.EditModel(user));
            }
        }

        [HttpPost]
        public ActionResult Edit(Models.Bill.BillEditModel model)
        {
            var user = _UserManager.GetUser(CurrentUser.Uid);
            try
            {
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
                return View(new Models.Bill.EditModel(user, model)
                {
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult Del(long bid)
        {
            var user = _UserManager.GetUser(CurrentUser.Uid);
            var bill = _BillManager.GetBill(user, bid);
            bill.Delete();
            return JsonResult(ErrorCode.None, "删除成功");
        }

        [HttpGet]
        public ActionResult List(Models.Bill.ListModel.SearchModel query)
        {
            var pageSize = 30;

            var user = _UserManager.GetUser(CurrentUser.Uid);
            var result = _BillManager.Search(user, query);

            double count = result.Count();
            var models = result
                            .OrderByDescending(b => b.bid)
                            .TakePage(query.PageIndex, pageSize)
                            .ToArray();

            var uids = models.SelectMany(b => b.agent_uid.HasValue ? new[] { b.creater, b.agent_uid.Value } : new[] { b.creater }).ToArray();
            var names = _UserManager.GetUsersName(uids);

            var model = new Models.Bill.ListModel
            {
                Query = query,
                Items = new Web.Core.PageModel<Models.Bill.ListModel.ListItemModel>(models.Select(m => new Models.Bill.ListModel.ListItemModel
                {
                    Bid = m.bid,
                    AgentName = (m.agent_uid.HasValue ? names.GetOrDefault(m.agent_uid.Value) : null) ?? "--",
                    CreaterName = names.GetOrDefault(m.creater) ?? "--",
                    Created = m.bill_date,
                    No = m.no,
                    ReceiverAddr = m.receiver_addr,
                    ReceiverName = m.receiver,
                    StateName = m.state == BillStates.None ? "--" : m.state.ToString(),
                }), query.PageIndex, (int)Math.Ceiling(count / (double)pageSize))
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Track(long bid)
        {
            var user = _UserManager.GetUser(CurrentUser.Uid);
            var bill = _BillManager.GetBill(user, bid);
            return ViewTask(bill);
        }

        [HttpPost]
        public ActionResult Track(Models.Bill.TrackEditModel model)
        {
            var user = _UserManager.GetUser(CurrentUser.Uid);
            var bill = _BillManager.GetBill(user, model.Bid);
            bill.UpdateState(model.State, model.Remarks, !model.UpdateState, model.Created);
            return RedirectToAction("Track", new { bid = model.Bid });
        }

        private ActionResult ViewTask(IBill bill)
        {
            var model = new Models.Bill.TrackModel
            {
                Bid = bill.Bid,
                No = bill.No,
                State = bill.State,
                Histroys = bill
                            .Histories
                            .OrderByDescending(h => h.state_updated)
                            .Select(h => new Models.Bill.TrackModel.HistoryItem
                            {
                                Bhid = h.bhid,
                                Created = h.state_updated,
                                Remarks = h.remarks,
                                State = h.state
                            }).ToArray()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult DelHistory(long bid, long bhid)
        {
            var user = _UserManager.GetUser(CurrentUser.Uid);
            var bill = _BillManager.GetBill(user, bid);
            bill.DeleteStateHistory(bhid);
            return JsonResult(ErrorCode.None, "删除成功");
        }
    }
}
