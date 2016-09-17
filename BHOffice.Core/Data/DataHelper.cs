using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Data
{
    public static class DataHelper
    {
        public static string GetSafeDbString(this string self)
        {
            if (self == null)
                return null;

            return System.Net.WebUtility.HtmlEncode(self);
        }
    }
}
