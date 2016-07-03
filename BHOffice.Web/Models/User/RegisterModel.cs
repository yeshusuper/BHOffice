using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.User
{
    public class RegisterModel : ErrorModel
    {
        public string UserNo { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}