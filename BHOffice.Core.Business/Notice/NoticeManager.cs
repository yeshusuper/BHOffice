using BHOffice.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Notice
{
    public interface INoticeManager
    {
        INotice GetNotice(long nid);
        INotice CreateNotice(IUser user, string title, string content);
        IQueryable<Data.Notice> EnabledList { get; }
    }

    internal class NoticeManager : INoticeManager
    {
        private readonly IRepository<Data.Notice> _NoticeRepository;

        public NoticeManager(IRepository<Data.Notice> noticeRepository)
        {
            _NoticeRepository = noticeRepository;
        }
        public IQueryable<Data.Notice> EnabledList
        {
            get { return _NoticeRepository.Entities.Where(n => n.enabled); }
        }

        public INotice CreateNotice(IUser user, string title, string content)
        {
            ExceptionHelper.ThrowIfNull(user, "user");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(title, "title", "标题不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(content, "content", "内容不能为空");

            if(user.Role < UserRoles.Admin)
                throw new BHException(ErrorCode.NotAllow, "没有操作权限");

            var entity = new Data.Notice
            {
                content = content.Trim().GetSafeDbString(),
                created = DateTime.Now,
                enabled = true,
                title = title.Trim().GetSafeDbString(),
                top = false,
                uid = user.Uid,
                updated = DateTime.Now,
            };
            _NoticeRepository.Add(entity);
            _NoticeRepository.SaveChanges();

            return new NoticeService(entity, _NoticeRepository);
        }

        public INotice GetNotice(long nid)
        {
            ExceptionHelper.ThrowIfNotId(nid, "nid");
            return new NoticeService(nid, _NoticeRepository);
        }
    }
}
