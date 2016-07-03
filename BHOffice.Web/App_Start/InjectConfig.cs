using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace BHOffice.Web
{
    public class InjectConfig
    {
        public static void Register()
        {            
            DependencyResolver.SetResolver(new Core.NinjectDependencyResolver(
                new BHOffice.Core.Business.Inject.DataModule(), 
                new BHOffice.Core.Business.Inject.ServiceModule()));
        }
    }
}