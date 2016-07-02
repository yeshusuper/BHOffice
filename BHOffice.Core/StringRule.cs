﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BHOffice.Core
{
    public static class StringRule
    {
        public static bool VerifyPassword(string password)
        {
            if (String.IsNullOrWhiteSpace(password))
                return false;
            password = password.Trim();
            if (password.Length < 6 || password.Length > 20)
                return false;
            return true;
        }

        public static bool VerifyEmail(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
                return false;
            email = email.Trim();
            var rx = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            return rx.IsMatch(email);
        }
    }
}
