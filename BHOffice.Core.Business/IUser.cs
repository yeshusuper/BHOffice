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
        string Name { get; }
        UserRoles Role { get; }
        void ResetPassword(string oldPwd, string newPwd);
        void UpdateInfo(IUserInfoEditer editer);

        void UpdateAgent(IUser user, bool open);
        void UpdateLock(IUser user, bool enabled);
    }

    class UserService : IUser
    {
        private readonly long _Uid;
        private readonly Lazy<Data.User> _LazyUser;
        private readonly Core.Data.IRepository<Data.User> _UserRepository;
        public long Uid { get { return _Uid; } }

        public UserService(long uid, Core.Data.IRepository<Data.User> userRepository)
            :this(userRepository)
        {
            _Uid = uid;
            _LazyUser = new Lazy<Data.User>(() =>
            {
                var entity = userRepository.Entities.FirstOrDefault(u => u.uid == _Uid);
                ExceptionHelper.ThrowIfNull(entity, "uid", "账号不存在");
                return entity;
            });
        }
        public UserService(Data.User entity, Core.Data.IRepository<Data.User> userRepository)
            : this(userRepository)
        {
            _Uid = entity.uid;
            _LazyUser = new Lazy<Data.User>(() => entity);
        }

        private UserService(Core.Data.IRepository<Data.User> userRepository)
        {
            _UserRepository = userRepository;
        }

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
            ExceptionHelper.ThrowIfNull(editer, "editer");
            editer.Fill(_LazyUser.Value);
            _UserRepository.SaveChanges();
        }

        public UserRoles Role
        {
            get { return _LazyUser.Value.role; }
        }


        public string Name
        {
            get { return _LazyUser.Value.name; }
        }


        public void UpdateAgent(IUser user, bool open)
        {
            ExceptionHelper.ThrowIfNull(user, "user");
            ExceptionHelper.ThrowIfNull(user as UserService, "user not userservice");

            if (Role < UserRoles.Admin)
                throw new BHException(ErrorCode.NotAllow, "没有操作权限");

            if (open && user.Role >= UserRoles.Agent)
                return;
            if (!open && user.Role < UserRoles.Agent)
                return;

            ((UserService)user)._LazyUser.Value.role = open ? UserRoles.Agent : UserRoles.User;
            _UserRepository.SaveChanges();
        }

        public void UpdateLock(IUser user, bool enabled)
        {
            ExceptionHelper.ThrowIfNull(user, "user");
            ExceptionHelper.ThrowIfNull(user as UserService, "user not userservice");

            if (Role < UserRoles.Admin)
                throw new BHException(ErrorCode.NotAllow, "没有操作权限");

            UserService iUser = user as UserService;

            if (enabled == iUser._LazyUser.Value.enabled)
                return;

            iUser._LazyUser.Value.enabled = enabled;
            _UserRepository.SaveChanges();
        }
    }

}
