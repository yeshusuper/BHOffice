using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHOffice.Core.Business.Kuaidi100
{
    public enum Kuaidi100ResultStatus : byte
    {
        //无物流信息
        None = 0,
        //成功返回
        Success = 1,
        //出错
        Error = 2,
    }
}
