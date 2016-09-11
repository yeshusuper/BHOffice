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
            var ids = model.Bids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(id => Convert.ToInt64(id)).ToArray();
            if (model.UpdateState)
                _BillAppService.UpdateState(CurrentUser.Uid, ids, model.State, model.Remarks, model.Created);
            else
                _BillAppService.InsertStateHistory(CurrentUser.Uid, ids, model.State, model.Remarks, model.Created);

            return SuccessJsonResult();
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

        [HttpPost]
        [BHAuthorize]
        public ActionResult ConfirmImportInternalNo()
        {
            if (Request.Files.Count == 0)
                return Content("没有上传文件");
            else
            {
                var file = Request.Files[0];
                if (file.ContentLength == 0)
                    return Content("不能上传空文件");
                else if (System.IO.Path.GetExtension(file.FileName).ToLower() != ".csv")
                    return Content("上传文件格式不正确，应上传csv文件");
                else
                {
                    var items = new List<Models.Bill.ConfirmImportInternalNoModel.Item>();
                    using (var reader = new System.IO.StreamReader(file.InputStream, System.Text.Encoding.GetEncoding("GB2312")))
                    {
                        reader.ReadLine();//标题；
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (!String.IsNullOrWhiteSpace(line))
                            {
                                var arr = line.Split(',');
                                var item = new Models.Bill.ConfirmImportInternalNoModel.Item
                                {
                                    No = GetImportCol(arr, 0),
                                    InternalNo = GetImportCol(arr, 1),
                                    InternalExpress = GetImportCol(arr, 2),
                                    Remarks = GetImportCol(arr, 3)
                                };
                                var stateDateString = GetImportCol(arr, 4);
                                if (!String.IsNullOrWhiteSpace(stateDateString))
                                {
                                    var stateDate = DateTime.MinValue;
                                    if (DateTime.TryParse(stateDateString, out stateDate))
                                    {
                                        item.UpdateStateDate = stateDate;
                                    }
                                }
                                if (item.No != null)
                                    items.RemoveAll(i => i.No == item.No);
                                items.Add(item);
                            }
                        }
                    }
                    if (items.Count == 0)
                        return Content("文件中不包含数据");

                    var nos = items.Where(i => !String.IsNullOrEmpty(i.No)).Select(i => i.No).Distinct().ToArray();
                    if(nos.Length > 0)
                    {
                        var user = _UserManager.GetUser(CurrentUser.Uid);
                        var existsNos = _BillManager.Search(user, null).Where(b => nos.Contains(b.no)).Select(b => new { b.no, b.bid }).ToArray();
                        foreach (var item in items)
                        {
                            if (item.No != null)
                                item.Bid = existsNos.Where(e => e.no == item.No).Select(e => e.bid).FirstOrDefault();
                        }
                    }
                    return View(new Models.Bill.ConfirmImportInternalNoModel
                    {
                        Items = items.ToArray()
                    });
                }
            }
        }

        [HttpPost]
        [BHAuthorize]
        public ActionResult ImportInternalNo(string model)
        {
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Bill.ConfirmImportInternalNoModel.Item[]>(model);
            var batchInternalTrade = new BatchInternalTrade();
            foreach (var item in data)
            {
                batchInternalTrade[item.Bid] = new BatchInternalTrade.Item(new InternalTrade(item.InternalNo, item.InternalExpress), item.Remarks, item.UpdateStateDate);
            }
            if (batchInternalTrade.Count > 0)
                _BillAppService.UpdateInternalState(CurrentUser.Uid, batchInternalTrade);
            return SuccessJsonResult();
        }

        [ChildActionOnly]
        private string GetImportCol(string[] arr, int index)
        {
            if (arr != null && arr.Length > index)
            {
                var val = arr[index];
                if(val != null){
                    val = val.Trim();
                    if (val.StartsWith("\"") && val.EndsWith("\""))
                    {
                        val = val.Substring(1, val.Length - 2);
                    }
                }
                return val;
            }
            return null;
        }
    }
}
