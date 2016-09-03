using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Web.Models.Bill
{
    public class BillEditModel
    {
        public long Bid { get; set; }

        #region  IBillArgs
        public string Sender { get; set; }

        public string SenderTel { get; set; }

        public string Receiver { get; set; }

        public string ReceiverTel { get; set; }

        public string ReceiverAddress { get; set; }

        public string No { get; set; }

        [DisplayFormat(NullDisplayText = "", DataFormatString = "yyyy/MM/dd HH:mm:ss")]
        public DateTime? Created { get; set; }

        public long? AgentUid { get; set; }

        public string Post { get; set; }

        public decimal Insurance { get; set; }

        public string Goods { get; set; }

        public string Remarks { get; set; }

        public string InternalExpress { get; set; }

        public string InternalNo { get; set; }
        #endregion

        public BillEditModel(BHOffice.Core.Business.Bill.IBill service)
            : this()
        {
            this.AgentUid = service.AgentUid;
            this.Bid = service.Bid;
            this.Created = service.Created;
            this.Goods = service.Goods;
            this.Insurance = service.Insurance;
            this.InternalExpress = service.InternalTrade.Express;
            this.InternalNo = service.InternalTrade.No;
            this.No = service.No;
            this.Post = service.Receiver.Address.Post;
            this.Receiver = service.Receiver.Name;
            this.ReceiverAddress = service.Receiver.Address.Addr;
            this.ReceiverTel = service.Receiver.Mobile;
            this.Remarks = service.Remarks;
            this.Sender = service.Sender.Name;
            this.SenderTel = service.Sender.Mobile;
        }

        public BillEditModel() { }
    }
}
