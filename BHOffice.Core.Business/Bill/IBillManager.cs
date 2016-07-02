using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBillManager
    {
        IBill Create(IUser user, IBillArgs args);
    }

    class BillManager : IBillManager
    {
        private readonly Core.Data.IRepository<Data.Bill> _BillRepository;

        public BillManager(Core.Data.IRepository<Data.Bill> billRepository)
        {
            _BillRepository = billRepository;
        }

        public IBill Create(IUser user, IBillArgs args)
        {
            ExceptionHelper.ThrowIfNull(user, "user");
            ExceptionHelper.ThrowIfNull(args, "args");

            ExceptionHelper.ThrowIfNullOrWhiteSpace(args.Sender, "sender", "发件人不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(args.SenderTel, "senderTel", "发件人电话不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(args.Receiver, "receiver", "收件人不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(args.ReceiverTel, "receiverTel", "收件人电话不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(args.ReceiverAddress, "receiverAddress", "收件人地址不能为空");

            var entity = new Data.Bill
            {
                no = args.No.SafeTrim(),
                sender = args.Sender.Trim(),
                sender_tel = args.SenderTel.Trim(),
                receiver = args.Receiver.Trim(),
                receiver_tel = args.ReceiverTel.Trim(),
                receiver_addr = args.ReceiverAddress.Trim(),
                post = args.Post.SafeTrim(),
                agent_uid = args.AgentUid,
                bill_date = args.Created ?? DateTime.Now,
                creater = user.Uid,
                goods = args.Goods.SafeTrim(),
                i_express = args.InternalExpress.SafeTrim(),
                i_no = args.InternalNo.SafeTrim(),
                insurance = args.Insurance,
                remarks = args.Remarks.SafeTrim(),
                updated = DateTime.Now,
                created = DateTime.Now,
            };
            if (String.IsNullOrWhiteSpace(entity.no))
                entity.no = CreateBillNo();

            _BillRepository.Add(entity);
            _BillRepository.SaveChanges();

            return new BillService(entity);
        }

        private string CreateBillNo()
        {
            return String.Format("{0:yyyyMMddHHmmssfff}{1}", DateTime.Now, new Random().Next(1, 100000).ToString().PadLeft(5, '0'));
        }
    }

}
