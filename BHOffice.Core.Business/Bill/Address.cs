using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public class Address
    {
        public string Addr { get; private set; }
        public string Post { get; private set; }

        public Address(string address, string post)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(address, "address");
            this.Addr = address.Trim();
            this.Post = (post ?? String.Empty).Trim();
        }
    }
}
