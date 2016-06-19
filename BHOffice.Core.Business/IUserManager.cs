using System;
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
    }
}
