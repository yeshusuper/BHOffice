using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business
{
    public interface IUser
    {
        long Uid { get; }
        UserRoles Role { get; }
        void ResetPassword(string oldPwd, string newPwd);
        void UpdateInfo(IUserInfoEditer editer);
    }

    class UserService : IUser
    {
        private readonly Lazy<Data.User> _LazyUser;
        private readonly Core.Data.IRepository<Data.User> _UserRepository;

        public UserService(long uid) { }
        public UserService(Data.User entity) { }

        public void ResetPassword(string oldPwd, string newPwd)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(oldPwd, "oldPwd", "旧密码不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(newPwd, "newPwd", "新密码不能为空");
            oldPwd = oldPwd.Trim();
            newPwd = newPwd.Trim();
            ExceptionHelper.ThrowIfTrue(!UserManager.VerifyPassword(oldPwd, _LazyUser.Value.pwd), "oldPwd", "旧密码不正确");
            _LazyUser.Value.pwd = UserManager.EncryptPassword(newPwd);
            _UserRepository.SaveChanges();
        }

        public void UpdateInfo(IUserInfoEditer editer)
        {
            throw new NotImplementedException();
        }

        public long Uid
        {
            get { throw new NotImplementedException(); }
        }

        
    }

}
