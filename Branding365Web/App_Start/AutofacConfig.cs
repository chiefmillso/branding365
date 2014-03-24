using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Branding365Web.Services;
using Microsoft.SharePoint.Client;

namespace Branding365Web
{
    public class AutofacConfig
    {
        public static void RegisterAutofac(HttpApplication httpApplication)
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly)
                .AssignableTo<IResolvableService>()
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();


            builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly)
                .AssignableTo<ISingletonService>()
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            builder.RegisterControllers(typeof(MvcApplication).Assembly)
                .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues);

            builder.RegisterFilterProvider();

            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}