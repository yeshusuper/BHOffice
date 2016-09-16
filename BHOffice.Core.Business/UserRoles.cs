using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business
{
    public enum UserRoles : byte
    {
        User = 0,
        Agent = 10,
        Admin = 20,
    }

    public static class UserRolesHelper
    {
        public static string GetName(this UserRoles role)
        {
            if (role >= UserRoles.Admin)
                return "管理员";
            else if (role >= UserRoles.Agent)
                return "代理商";
            else
                return "用户";
        }

        public static void GetRange(this UserRoles role, out UserRoles min, out UserRoles? max)
        {
            if (role >= UserRoles.Admin)
            {
                min = UserRoles.Admin;
                max = null;
            }
            else if(role >= UserRoles.Agent) 
            {
                min = UserRoles.Agent;
                max = UserRoles.Admin;
            }
            else
            {
                min = UserRoles.User;
                max = UserRoles.Agent;
            }
        }
    }
}
