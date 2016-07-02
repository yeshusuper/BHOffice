﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business
{
    public interface IUserManager
    {
        IUser Login(string userNo, string password);
        IUser GetUser(long uid);
        IUser Register(string userNo, string password, string username);
        bool IsUsable(ref string userNo);
    }

    class UserManager : IUserManager
    {
        private readonly Data.IRepository<Data.User> _UserRepository;

        public UserManager(Data.IRepository<Data.User> userRepository)
        {
            _UserRepository = userRepository;
        }

        public IUser Login(string userNo, string password)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(userNo, "userNo", "账号不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(password, "password", "密码不能为空");
            userNo = userNo.Trim();
            password = password.Trim();

            var entity = _UserRepository.Entities.FirstOrDefault(u => u.email == userNo);
            if (entity == null)
                throw new BHException(ErrorCore.NotExists, "账号不存在");
            if (!entity.enabled)
                throw new BHException(ErrorCore.Locked, "账号被锁定");
            if (!new Security.MD5().Verify(password, entity.pwd))
                throw new BHException(ErrorCore.ErrorUserNoOrPwd, "账号或密码错误");

            return new UserService(entity);
        }

        public IUser GetUser(long uid)
        {
            ExceptionHelper.ThrowIfNotId(uid, "uid");
            return new UserService(uid);
        }

        public IUser Register(string userNo, string password, string username)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(password, "password", "密码不能为空");
            ExceptionHelper.ThrowIfNullOrWhiteSpace(username, "username", "用户名不能为空");
            password = password.Trim();
            username = username.Trim();
            ExceptionHelper.ThrowIfTrue(!StringRule.VerifyPassword(password), "password", "密码格式不正确");
            ExceptionHelper.ThrowIfTrue(username.Length < 2 || username.Length > 6, "username", "请填写长度为2到6位的用户名");
            ExceptionHelper.ThrowIfTrue(!IsUsable(ref userNo), "userNo", "该账号已经被注册");

            var entity = new Data.User
            {
                email = userNo,
                enabled = true,
                name = username,
                pwd = new Security.MD5().Encrypt(password),
                registed = DateTime.Now,
                role = UserRoles.User,
            };
            _UserRepository.Add(entity);
            _UserRepository.SaveChanges();

            return new UserService(entity);
        }


        public bool IsUsable(ref string userNo)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(userNo, "userNo", "账号不能为空");
            ExceptionHelper.ThrowIfTrue(!StringRule.VerifyEmail(userNo), "userNo", "请填写邮件作为账号");
            userNo = userNo.Trim();
            var un = userNo;
            return !_UserRepository.Entities.Any(u => u.email == un);
        }
    }

}