using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Bill
{
    public class ContactInfo
    {
        public string Name { get; private set; }
        public Mobile Mobile { get; private set; }

        public ContactInfo(string name, Mobile mobile)
        {
            ExceptionHelper.ThrowIfNullOrWhiteSpace(name, "name");
            ExceptionHelper.ThrowIfNull(mobile, "mobile");

            this.Name = name.Trim();
            this.Mobile = mobile;
        }
    }

    public class ContactInfoWithAddress : ContactInfo
    {
        public Address Address { get; private set; }

        public ContactInfoWithAddress(string name, Mobile mobile, Address address)
            : base(name, mobile)
        {
            ExceptionHelper.ThrowIfNull(address, "address");
            this.Address = address;
        }
    }
}
