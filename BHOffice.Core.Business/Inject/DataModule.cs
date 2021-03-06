﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace BHOffice.Core.Business.Inject
{
    public class DataModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<Core.Data.IRepository<Data.Notice>>().To<Data.DbSetRepository<Data.BHOfficeContext, Data.Notice>>();
            Bind<Core.Data.IRepository<Data.User>>().To<Data.DbSetRepository<Data.BHOfficeContext, Data.User>>();
            Bind<Data.IBillRepository>().To<Data.BillRepository>();
            Bind<Core.Data.IRepository<Data.BillStateHistory>>().To<Data.DbSetRepository<Data.BHOfficeContext, Data.BillStateHistory>>();
        }
    }
}
