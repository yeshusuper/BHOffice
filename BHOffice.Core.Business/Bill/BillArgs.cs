using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public class BillArgs
    {
        private string _Sender;
        public string Sender
        {
            get { return _Sender; }
            set 
            {
                ExceptionHelper.ThrowIfNullOrWhiteSpace(_Sender, "sender", "发件人不能为空");
                _Sender = value.Trim(); 
            }
        }

        private string _SenderTel;
        public string SenderTel
        {
            get { return _SenderTel; }
            set
            {
                ExceptionHelper.ThrowIfNullOrWhiteSpace(_SenderTel, "senderTel", "发件人电话不能为空");
                _SenderTel = value.Trim();
            }
        }

        private string _Receiver;
        public string Receiver
        {
            get { return _Receiver; }
            set
            {
                ExceptionHelper.ThrowIfNullOrWhiteSpace(_Receiver, "receiver", "收件人不能为空");
                _Receiver = value.Trim();
            }
        }

        private string _ReceiverTel;
        public string ReceiverTel
        {
            get { return _ReceiverTel; }
            set
            {
                ExceptionHelper.ThrowIfNullOrWhiteSpace(_ReceiverTel, "receiverTel", "收件人电话不能为空");
                _ReceiverTel = value.Trim();
            }
        }

        private string _ReceiverAddress;
        public string ReceiverAddress
        {
            get { return _ReceiverAddress; }
            set
            {
                ExceptionHelper.ThrowIfNullOrWhiteSpace(_ReceiverAddress, "receiverAddress", "收件人地址不能为空");
                _ReceiverAddress = value.Trim();
            }
        }
        /// <summary>
        /// 单号
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? Created { get; set; }
        /// <summary>
        /// 代理商
        /// </summary>
        public long? AgentUid { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Post { get; set; }
        /// <summary>
        /// 保险
        /// </summary>
        public decimal Insurance { get; set; }
        /// <summary>
        /// 物品
        /// </summary>
        public string Goods { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 国内快递
        /// </summary>
        public string InternalExpress { get; set; }
        /// <summary>
        /// 国内快递单号
        /// </summary>
        public string InternalNo { get; set; }


        public BillArgs(string sender, string senderTel, string receiver, string receiverTel, string receiverAddress)
        {
            Sender = sender;
            SenderTel = senderTel;
            Receiver = receiver;
            ReceiverTel = receiverTel;
            ReceiverAddress = receiverAddress;
        }
    }
}
