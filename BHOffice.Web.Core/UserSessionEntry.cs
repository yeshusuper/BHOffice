using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Web.Core
{
    [Serializable]
    public class UserSessionEntry
    {
        public long Uid { get; set; }
        public string Name { get; set; }
    }
}
