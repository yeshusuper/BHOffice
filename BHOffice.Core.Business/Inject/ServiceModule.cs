﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace BHOffice.Core.Business.Inject
{
    public class ServiceModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<Notice.INoticeManager>().To<Notice.NoticeManager>();
            Bind<IUserManager>().To<UserManager>();
            Bind<Bill.IBillManager>().To<Bill.BillManager>();
            Bind<Bill.IBillAppService>().To<Bill.BillAppService>();
        }
    }
}
