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
    }
}
