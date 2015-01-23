using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using OpenBank.Core;
using OpenBank.Web.Infrastructure.Windsor;

namespace OpenBank.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        private IWindsorContainer m_Container;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles); 
            
            m_Container = BootstrapContainer();

            // TODO[tv]: It may be better to use Castle startable facility.
            //          Just don't want to bring a dependency on IoC to Core module for now.
            m_Container.Resolve<DataProducer>().Start();
            m_Container.Resolve<StoringDataConsumer>().Start();
        }

        protected void Application_End()
        {
            if (m_Container != null)
            {
                m_Container.Dispose();
                m_Container = null;
            }
        }

        internal static IWindsorContainer BootstrapContainer()
        {
            var container = new WindsorContainer();
            container.Install(new ComponentsInstaller());
            return container;
        }
    }
}