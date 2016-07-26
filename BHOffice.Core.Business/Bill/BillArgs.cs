﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public interface IBillUpdateStrategy
    {
        bool IsReadOnly { get; }
        bool IsSenderAndReceiverReadOnly { get; }
        bool IsAllowUpdateState { get; }
        /// <summary>
        /// 是否允许修改运单时间
        /// </summary>
        bool IsAllowUpdateCreated { get; }
        /// <summary>
        /// 是否允许修改代理商
        /// </summary>
        bool IsAllowUpdateAgent { get; }
    }

    internal class AllAllowBillUpdateStrategy : IBillUpdateStrategy
    {
        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsSenderAndReceiverReadOnly
        {
            get { return false; }
        }

        public bool IsAllowUpdateState
        {
            get { return false; }
        }


        public bool IsAllowUpdateCreated { get; private set; }

        public bool IsAllowUpdateAgent { get; private set; }

        public AllAllowBillUpdateStrategy(IUser user)
        {
            IsAllowUpdateCreated = user.Role >= UserRoles.Agent;
            IsAllowUpdateAgent = user.Role >= UserRoles.Admin;
        }
    }


    public interface IBillArgs
    {
        string Sender { get; }
        string SenderTel { get; }
        string Receiver { get; }
        string ReceiverTel { get; }
        string ReceiverAddress { get; }
        /// <summary>
        /// 单号
        /// </summary>
        string No { get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime? Created { get; }
        /// <summary>
        /// 代理商
        /// </summary>
        long? AgentUid { get; }
        /// <summary>
        /// 邮编
        /// </summary>
        string Post { get; }
        /// <summary>
        /// 保险
        /// </summary>
        decimal Insurance { get; }
        /// <summary>
        /// 物品
        /// </summary>
        string Goods { get; }
        /// <summary>
        /// 备注
        /// </summary>
        string Remarks { get; }
        /// <summary>
        /// 国内快递
        /// </summary>
        string InternalExpress { get; }
        /// <summary>
        /// 国内快递单号
        /// </summary>
        string InternalNo { get; }
    }

    static class BillArgsHelper
    {
        public static void Verify(this IBillArgs args, IBillUpdateStrategy strategy)
        {
            ExceptionHelper.ThrowIfNull(args, "args");

            if (!strategy.IsSenderAndReceiverReadOnly)
            {
                ExceptionHelper.ThrowIfNullOrWhiteSpace(args.Sender, "sender", "发件人不能为空");
                ExceptionHelper.ThrowIfNullOrWhiteSpace(args.SenderTel, "senderTel", "发件人电话不能为空");
                ExceptionHelper.ThrowIfNullOrWhiteSpace(args.Receiver, "receiver", "收件人不能为空");
                ExceptionHelper.ThrowIfNullOrWhiteSpace(args.ReceiverTel, "receiverTel", "收件人电话不能为空");
                ExceptionHelper.ThrowIfNullOrWhiteSpace(args.ReceiverAddress, "receiverAddress", "收件人地址不能为空");
            }
        }

        public static void Fill(this IBillArgs args, IBillUpdateStrategy strategy, Data.Bill entity, IUser user)
        {
            if (!strategy.IsSenderAndReceiverReadOnly)
            {
                entity.sender = args.Sender.Trim();
                entity.sender_tel = args.SenderTel.Trim();
                entity.receiver = args.Receiver.Trim();
                entity.receiver_tel = args.ReceiverTel.Trim();
                entity.receiver_addr = args.ReceiverAddress.Trim();
            }
            if (!strategy.IsReadOnly)
            {
                entity.no = args.No.SafeTrim();
                entity.post = args.Post.SafeTrim();
                entity.goods = args.Goods.SafeTrim();
                entity.i_express = args.InternalExpress.SafeTrim();
                entity.i_no = args.InternalNo.SafeTrim();
                entity.insurance = args.Insurance;
                entity.remarks = args.Remarks.SafeTrim();
                entity.updated = DateTime.Now;
                entity.updater = user.Uid;

                if(strategy.IsAllowUpdateCreated)
                {
                    entity.bill_date = args.Created ?? DateTime.Now;
                }

                if (strategy.IsAllowUpdateAgent)
                {
                    entity.agent_uid = args.AgentUid ?? user.Uid;
                }
                else if (!entity.agent_uid.HasValue && user.Role >= UserRoles.Agent)
                {
                    entity.agent_uid = user.Uid;
                }

                if(!entity.confirmed && user.Role >= UserRoles.Agent)
                {
                    entity.confirmed = true;
                    entity.confirmer = user.Uid;
                }
            }
        }
    }
}
