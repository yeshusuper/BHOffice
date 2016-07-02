using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core
{
    public static class StringHelper
    {
        public static string SafeTrim(this string str)
        {
            if (str == null)
                return null;
            return str.Trim();
        }
    }
}
