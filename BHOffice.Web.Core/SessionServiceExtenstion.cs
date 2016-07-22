using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BHOffice.Web.Core
{
    public static class SessionServiceExtenstion
    {
        public static UserSessionEntry GetCurrentUser(this HttpSessionStateBase session)
        {
            return session[Config.Current.SESSION_USER_KEY] as UserSessionEntry;
        }

        public static void SetCurrentUser(this HttpSessionStateBase session, UserSessionEntry entry)
        {
            session[Config.Current.SESSION_USER_KEY] = entry;
        }



        public static UserSessionEntry Login(this HttpSessionStateBase session, BHOffice.Core.Business.IUserManager manager, string username, string password)
        {
            var user = manager.Login(username, password);
            var entry = new UserSessionEntry
            {
                Name = user.Name,
                Uid = user.Uid
            };
            SetCurrentUser(session, entry);
            return entry;
        }
    }
}
