using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business
{
    public interface IUser
    {
        void ResetPassword(string oldPwd, string newPwd);
        void UpdateInfo(IUserInfoEditer editer);
    }
}
