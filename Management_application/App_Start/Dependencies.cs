using Autofac;
using Autofac.Integration.Mvc;
using Management_application.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Management_application.App_Start
{

        public static class Dependencies
        {
            public static void Register()
            {
                var builder = new ContainerBuilder();
                builder.RegisterControllers(typeof(MvcApplication).Assembly);

                builder.RegisterType<SqlRepository>().As<Repository>().InstancePerRequest();

                var container = builder.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            }
        }
    }
