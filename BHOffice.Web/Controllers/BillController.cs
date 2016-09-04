using BHOffice.Core.Business;
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
    public class BillController : BaseController
    {
        private readonly IBillManager _BillManager;
        private readonly IUserManager _UserManager;
        private readonly IBillAppService _BillAppService;

        public BillController(IBillManager billManager,
            IUserManager userManager,
            IBillAppService billAppService)
        {
            _BillManager = billManager;
            _UserManager = userManager;
            _BillAppService = billAppService;
        }

        [HttpGet]
        [BHAuthorize]
        public ActionResult Edit(long? id)
        {
            var user = _UserManager.GetUser(CurrentUser.Uid);
            Models.Bill.EditModel model;
            var canSetAgent = false;
            if(id.HasValue && id.Value > 0)
            {
                var bill =_BillManager.GetBill(id.Value);
                var auth = new BillAuthority(user, bill);
                canSetAgent = auth.AllowSetAgent;
                if (auth.AllowView)
                   model = new Models.Bill.EditModel(auth, bill);
                else
                    throw new BHException(ErrorCode.NotAllow, "没有此运单查看权限");
            }
            else
            {
                canSetAgent = user.Role >= UserRoles.Admin;
                model = new Models.Bill.EditModel(user);
            }
            if(canSetAgent)
            {
                model.Agents = _UserManager.GetAgents()
                                    .OrderBy(a => a.uid)
                                    .Select(a => new { a.uid, a.name })
                                    .ToArray()
                                    .ToDictionary(a => a.uid, a => a.name);
            }
            return View(model);
        }

        [HttpPost]
        [BHAuthorize]
        public ActionResult Edit(Models.Bill.BillEditModel model)
        {
            var redirect = String.Empty;
            if (model.Bid > 0)
            {
                _BillAppService.UpdateBill(CurrentUser.Uid, model.Bid,
                    model.Sender, model.SenderTel,
                    model.Receiver, model.ReceiverTel, model.ReceiverAddress, model.Post,
                    model.Insurance, model.Goods, model.Remarks,
                    model.AgentUid, model.Created,
                    model.InternalNo, model.InternalExpress);
            }
            else
            {
                var bill = _BillAppService.CreateBill(CurrentUser.Uid,
                   model.Sender, model.SenderTel,
                   model.Receiver, model.ReceiverTel, model.ReceiverAddress, model.Post,
                   model.Insurance, model.Goods, model.Remarks,
                   model.AgentUid, model.Created,
                   model.InternalNo, model.InternalExpress);

                redirect = this.Url.Action("Edit", new { id = bill.Bid });
            }
            return SuccessJsonResult(redirect);
        }

        [HttpPost]
        [BHAuthorize]
        public ActionResult Del(long bid)
        {
            _BillAppService.DeleteBill(CurrentUser.Uid, bid);
            return SuccessJsonResult();
        }

        [HttpGet]
        [BHAuthorize]
        public ActionResult List(Models.Bill.ListModel.SearchModel query)
        {
            var pageSize = 30;
            var maxCreated = query.MaxCreated;
            if (query.MaxCreated.HasValue)
            {
                query.MaxCreated = query.MaxCreated.Value.AddDays(1);
            }

            var user = _UserManager.GetUser(CurrentUser.Uid);
            var result = _BillManager.Search(user, query);
            
            double count = result.Count();
            var models = result
                            .OrderByDescending(b => b.bid)
                            .TakePage(query.PageIndex, pageSize)
                            .ToArray();

            var uids = models.SelectMany(b => b.agent_uid.HasValue ? new[] { b.creater, b.agent_uid.Value } : new[] { b.creater }).ToArray();
            var names = _UserManager.GetUsersName(uids);

            query.MaxCreated = maxCreated;

            var model = new Models.Bill.ListModel(user)
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
                    IsDisplayDeleteButton = new BillAuthority(user, m).AllowDelete,
                }), query.PageIndex, (int)Math.Ceiling(count / (double)pageSize))
            };

            return View(model);
        }

        [HttpGet]
        [BHAuthorize]
        public ActionResult Track(long id)
        {
            var bill = _BillManager.GetBill(id);
            return ViewTask(bill);
        }

        [HttpPost]
        [BHAuthorize]
        public ActionResult Track(Models.Bill.TrackEditModel model)
        {
            if (model.UpdateState)
                _BillAppService.UpdateState(CurrentUser.Uid, model.Bid, model.State, model.Remarks, model.Created);
            else
                _BillAppService.InsertStateHistory(CurrentUser.Uid, model.Bid, model.State, model.Remarks, model.Created);

            return RedirectToAction("Track", new { id = model.Bid });
        }

        private ActionResult ViewTask(IBill bill)
        {
            var model = new Models.Bill.TrackModel
            {
                Bid = bill.Bid,
                No = bill.No,
                State = bill.State,
                Histroys = _BillManager.GetBillHistories(new []{ bill.Bid })
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
        [BHAuthorize]
        public ActionResult DelHistory(long bid, long bhid)
        {
            _BillAppService.DeleteBillStateHistory(CurrentUser.Uid, bid, bhid);
            return SuccessJsonResult();
        }

        [HttpPost]
        public ActionResult Way(string nos)
        {
            var noArr = nos
                        .Split(new[] { ",", " ", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Take(20)
                        .ToArray();

            var bills = _BillManager.GetBill(noArr)
                            .Select(b => new
                            {
                                b.bid,
                                b.no,
                                b.state,
                                b.i_express,
                                b.i_no
                            }).ToArray();
            var histories = _BillManager.GetBillHistories(bills.Select(b => b.bid).ToArray()).ToArray();

            return SuccessJsonResult(bills.Select(b => new Models.Bill.WayModel.WayItemModel
            {
                No = b.no,
                State = b.state,
                InternalExpress = b.i_express,
                InternalNo = b.i_no,
                Histories = histories.Where(h => h.bid == b.bid).OrderByDescending(h => h.created)
                                    .Select(h => new Models.Bill.TrackModel.HistoryItem
                                    {
                                        Bhid = h.bhid,
                                        Created = h.state_updated,
                                        Remarks = h.remarks,
                                        State = h.state
                                    }).ToArray()
            }).ToArray());
        }

        [HttpGet]
        [BHAuthorize]
        public ActionResult Print(long id)
        {
            var user = _UserManager.GetUser(CurrentUser.Uid);
            var bill = _BillManager.GetBill(id);
            var auth = new BillAuthority(user, bill);

            if (!auth.AllowView)
                throw new BHException(ErrorCode.NotAllow, "你没有查看此运单的权限");

            return View(new Models.Bill.BillEditModel(bill));
        }

        [HttpGet]
        public ActionResult Barcode(string id)
        {
            using(var barcode = new BarcodeLib.Barcode().Encode(BarcodeLib.TYPE.CODE39, id,
                                System.Drawing.Color.Black,
                                System.Drawing.Color.White,
                                500, 70))
            {
                var memory = new System.IO.MemoryStream();
                barcode.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                return File(memory, "image/png");
            }

        }
    }
}
