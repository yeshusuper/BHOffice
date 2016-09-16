using BHOffice.Core.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.User
{
    public class ListModel
    {
        public class SearchQuery : IUserSearchQuery
        {
            public string Name { get; set; }
            public UserRoles? Role { get; set; }
            public bool? IsEnabled { get; set; }
            public int Page { get; set; }
        }

        public class Item
        {
            public long Uid { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public UserRoles Role { get; set; }
            public DateTime Registered { get; set; }
            public bool IsEnabled { get; set; }

            public string StateName
            {
                get
                {
                    return IsEnabled ? "正常" : "锁定";
                }
            }
        }
        public BHOffice.Web.Core.PageModel<Item> Items { get; set; }
        public SearchQuery Query { get; set; }
    }
}