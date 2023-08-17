using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Forte.Web.React.React;
using Jering.Javascript.NodeJS;

namespace Forte.Web.React.Examples.Framework
{
    public class MvcApplication : HttpApplication
    {
        public static INodeJSService NodeJsService = new StaticNodeJsServiceProxy(); 
        
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            SetupDependencyResolver();
        }

        private static void SetupDependencyResolver()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            RegisterReactServices(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void RegisterReactServices(ContainerBuilder builder)
        {
            builder.RegisterInstance(ReactAssets.ReactConfig).SingleInstance();
            builder.RegisterType<StaticNodeJsServiceProxy>().As<INodeJSService>().SingleInstance();
            builder.RegisterType<ReactService>().As<IReactService>().InstancePerRequest();
        }
    }
}