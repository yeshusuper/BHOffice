using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Data
{
    public class User
    {
        public long uid { get; set; }
        public string email { get; set; }
        public string pwd { get; set; }
        public string name { get; set; }
        public DateTime registed { get; set; }
        public bool enabled { get; set; }
        public Business.UserRoles role { get; set; }
    }
}
