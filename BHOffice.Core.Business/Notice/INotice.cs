using BHOffice.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Notice
{
    public interface INotice
    {
        long Nid { get; }
        string Title { get; }
        string Content { get; }
        DateTime Date { get; }
        void Update(IUser @operator, string title, string content);
        void UpdateTop(IUser @operator, bool top);
        void UpdateEnabled(IUser @operator, bool enabled);
    }

    internal class NoticeService : INotice
    {
        private readonly IRepository<Data.Notice> _NoticeRepository;
        private readonly Lazy<Data.Notice> _LazyNotice;
        private readonly Func<long> _LazyNid;

        public NoticeService(Data.Notice entity, IRepository<Data.Notice> noticeRepository)
        {
            _LazyNotice = new Lazy<Data.Notice>(() => entity);
            _LazyNid = () => _LazyNotice.Value.nid;
            _NoticeRepository = noticeRepository;
        }
        public NoticeService(long nid, IRepository<Data.Notice> noticeRepository)
        {
            _LazyNid = () => nid;
            _LazyNotice = new Lazy<Data.Notice>(() =>
            {
                var entity = _NoticeRepository.Entities.FirstOrDefault(n => n.nid == nid);
                ExceptionHelper.ThrowIfNull(entity, "nid");
                return entity;
            });
            _NoticeRepository = noticeRepository;
        }

        public void Update(IUser @operator, string title, string content)
        {
            ExceptionHelper.ThrowIfNull(@operator, "@operator");
            if (@operator.Role < UserRoles.Admin)
                throw new BHException(ErrorCode.NotAllow, "没有操作权限");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(title, "title", "标题不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(content, "content", "内容不能为空");

            title = title.Trim().GetSafeDbString();
            content = content.Trim().GetSafeDbString();
            if (title != _LazyNotice.Value.title || content != _LazyNotice.Value.content)
            {
                _LazyNotice.Value.title = title;
                _LazyNotice.Value.content = content;
                _LazyNotice.Value.updated = DateTime.Now;
                _LazyNotice.Value.updater = @operator.Uid;
                _NoticeRepository.SaveChanges();
            }
        }

        public void UpdateTop(IUser @operator, bool top)
        {
            ExceptionHelper.ThrowIfNull(@operator, "@operator");
            if (@operator.Role < UserRoles.Admin)
                throw new BHException(ErrorCode.NotAllow, "没有操作权限");

            if (top != _LazyNotice.Value.top)
            {
                _LazyNotice.Value.top = top;
                _LazyNotice.Value.updated = DateTime.Now;
                _LazyNotice.Value.updater = @operator.Uid;
                _NoticeRepository.SaveChanges();
            }
        }

        public void UpdateEnabled(IUser @operator, bool enabled)
        {
            ExceptionHelper.ThrowIfNull(@operator, "@operator");
            if (@operator.Role < UserRoles.Admin)
                throw new BHException(ErrorCode.NotAllow, "没有操作权限");

            if (enabled != _LazyNotice.Value.enabled)
            {
                _LazyNotice.Value.enabled = enabled;
                _LazyNotice.Value.updated = DateTime.Now;
                _LazyNotice.Value.updater = @operator.Uid;
                _NoticeRepository.SaveChanges();
            }
        }

        public long Nid
        {
            get { return _LazyNid(); }
        }

        public string Title
        {
            get { return _LazyNotice.Value.title; }
        }

        public string Content
        {
            get { return _LazyNotice.Value.content; }
        }

        public DateTime Date
        {
            get { return _LazyNotice.Value.updated; }
        }
    }
}
