﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHOffice.Core.Business.Data;

namespace BHOffice.Core.Business
{
    public interface IUserManager
    {
        IUser Login(string userNo, string password);
        IUser GetUser(long uid);
        IUser Register(string userNo, string password, string username);
        void ResetPassword(long uid, string oldPassword, string newPassword);
        bool IsUsable(ref string userNo);
        Dictionary<long, string> GetUsersName(long[] uids);
        IQueryable<Data.User> GetAgents();
        IQueryable<Data.User> SearchUser(IUserSearchQuery query);
    }

    public interface IUserSearchQuery
    {      
        string Name { get; }
        UserRoles? Role { get; }
        bool? IsEnabled { get; }
    }

    class UserManager : IUserManager
    {
        private readonly Core.Data.IRepository<Data.User> _UserRepository;

        public UserManager(Core.Data.IRepository<Data.User> userRepository)
        {
            _UserRepository = userRepository;
        }


        public void ResetPassword(long uid, string oldPassword, string newPassword)
        {
            ExceptionHelper.ThrowIfNotId(uid, "uid");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(oldPassword, "oldPassword", "旧密码不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(newPassword, "newPassword", "新密码不能为空");
            oldPassword = oldPassword.Trim();
            newPassword = newPassword.Trim();
            ExceptionHelper.ThrowIfTrue(!StringRule.VerifyPassword(newPassword), "newPassword", "新密码格式不正确");

            var entity = _UserRepository.Entities.FirstOrDefault(u => u.uid == uid);
            ExceptionHelper.ThrowIfNull(entity, "uid", "账号不存在");

            if (!VerifyPassword(oldPassword, entity.pwd))
                throw new BHException(ErrorCode.ErrorUserNoOrPwd, "旧密码错误");

            entity.pwd = EncryptPassword(newPassword);
            _UserRepository.SaveChanges();
        }

        public IUser Login(string userNo, string password)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(userNo, "userNo", "账号不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(password, "password", "密码不能为空");
            userNo = userNo.Trim();
            password = password.Trim();

            var entity = _UserRepository.Entities.FirstOrDefault(u => u.email == userNo);
            if (entity == null)
                throw new BHException(ErrorCode.NotExists, "账号不存在");
            if (!entity.enabled)
                throw new BHException(ErrorCode.Locked, "账号被锁定");
            if (!VerifyPassword(password, entity.pwd))
                throw new BHException(ErrorCode.ErrorUserNoOrPwd, "账号或密码错误");

            return new UserService(entity, _UserRepository);
        }

        public IUser GetUser(long uid)
        {
            ExceptionHelper.ThrowIfNotId(uid, "uid");
            return new UserService(uid, _UserRepository);
        }

        public IUser Register(string userNo, string password, string username)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(password, "password", "密码不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(username, "username", "用户名不能为空");
            password = password.Trim();
            username = username.Trim();
            ExceptionHelper.ThrowIfTrue(!StringRule.VerifyPassword(password), "password", "密码格式不正确");
            ExceptionHelper.ThrowIfTrue(username.Length < 2 || username.Length > 6, "username", "请填写长度为2到6位的用户名");

            if (!IsUsable(ref userNo))
                throw new BHException(ErrorCode.Exists, "该账号已经被注册");
                

            var entity = new Data.User
            {
                email = userNo,
                enabled = true,
                name = username,
                pwd = EncryptPassword(password),
                created = DateTime.Now,
                role = UserRoles.User,
            };
            _UserRepository.Add(entity);
            _UserRepository.SaveChanges();

            return new UserService(entity, _UserRepository);
        }


        public bool IsUsable(ref string userNo)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(userNo, "userNo", "账号不能为空");
            ExceptionHelper.ThrowIfTrue(!StringRule.VerifyEmail(userNo), "userNo", "请填写邮件作为账号");
            userNo = userNo.Trim();
            var un = userNo;
            return !_UserRepository.Entities.Any(u => u.email == un);
        }

        internal static string EncryptPassword(string input)
        {
            return new Security.MD5().Encrypt(input);
        }
        internal static bool VerifyPassword(string input, string password)
        {
            return new Security.MD5().Verify(input, password);
        }

        public Dictionary<long, string> GetUsersName(long[] uids)
        {
            if (uids == null)
                return new Dictionary<long, string>();
            uids = uids.Where(id => id > 0).Distinct().ToArray();
            if (uids.Length == 0)
                return new Dictionary<long, string>();

            var result = _UserRepository.Entities.Where(u => uids.Contains(u.uid)).Select(u => new { u.uid, u.name }).ToArray();
            return result.ToDictionary(r => r.uid, r => r.name);
        }

        public IQueryable<User> GetAgents()
        {
            return _UserRepository.Entities.Where(u => u.enabled && u.role >= UserRoles.Agent);
        }

        public IQueryable<User> SearchUser(IUserSearchQuery query)
        {
            var source = _UserRepository.Entities;
            if (query != null)
            {
                if (!String.IsNullOrWhiteSpace(query.Name))
                {
                    var name = query.Name.Trim();
                    source = source.Where(u => u.email == name || u.name == name);
                }
                if (query.Role.HasValue)
                {
                    UserRoles min;
                    UserRoles? max;
                    query.Role.Value.GetRange(out min, out max);

                    source = source.Where(u => u.role >= min);
                    if (max.HasValue)
                        source = source.Where(u => u.role < max.Value);
                }
                if (query.IsEnabled.HasValue)
                    source = source.Where(u => u.enabled == query.IsEnabled.Value);
            }
            return source;
        }
    }

}
