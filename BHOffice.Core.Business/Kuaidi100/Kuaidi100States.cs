using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Kuaidi100
{
    public enum Kuaidi100States : byte
    {
        /// <summary>
        /// 在途，即货物处于运输过程中；
        /// </summary>
        Road = 0,
        /// <summary>
        /// 揽件，货物已由快递公司揽收并且产生了第一条跟踪信息；
        /// </summary>
        Get = 1,
        /// <summary>
        /// 疑难，货物寄送过程出了问题；
        /// </summary>
        Difficult = 2,
        /// <summary>
        /// 签收，收件人已签收；
        /// </summary>
        Sign = 3,
        /// <summary>
        /// 退签，即货物由于用户拒签、超区等原因退回，而且发件人已经签收；
        /// </summary>
        Deny = 4,
        /// <summary>
        /// 派件，即快递正在进行同城派件；
        /// </summary>
        Send = 5,
        /// <summary>
        /// 退回，货物正处于退回发件人的途中；
        /// </summary>
        Return = 6
    }
}
