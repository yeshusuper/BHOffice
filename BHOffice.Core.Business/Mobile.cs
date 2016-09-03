using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business
{
    public class Mobile
    {
        private string _InternalMobile;

        public Mobile(string mobile)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(mobile, "mobile");
            _InternalMobile = mobile.Trim();
        }

        public static implicit operator string(Mobile mobile)
        {
            return mobile == null ? null : mobile._InternalMobile;
        }

        public static implicit operator Mobile(string mobile)
        {
            return mobile == null ? null : new Mobile(mobile);
        }
    }
}
