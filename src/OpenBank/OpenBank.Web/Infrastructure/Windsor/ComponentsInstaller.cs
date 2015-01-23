using System.Globalization;
using System.Web.Configuration;
using System.Web.Mvc;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.AspNet.SignalR.Hubs;
using OpenBank.Core;
using OpenBank.Core.Data;
using OpenBank.Core.Persistence;
using OpenBank.Core.Transport;
using OpenBank.Web.Controllers;
using OpenBank.Web.Hubs;
using OpenBank.Web.Infrastructure.SignalR;

namespace OpenBank.Web.Infrastructure.Windsor
{
    public sealed class ComponentsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
            container.AddFacility<StartableFacility>();

            // Components
            var inMemoryStorage = bool.Parse(WebConfigurationManager.AppSettings["UseInMemoryStorage"]);
            if (inMemoryStorage)
            {
                container.Register(
                    Component.For<IDataStorage>().ImplementedBy<InMemoryDataStorage>()
                    );
            }
            else
            {
                container.Register(
                    Component.For<IDataStorage>()
                             .ImplementedBy<MSSQLDataStorage>()
                             .DependsOn(new { connectionString = WebConfigurationManager.ConnectionStrings["main"].ConnectionString })
                    );
            }
            container.Register(                
                Component.For<IDataGenerator>().ImplementedBy<TrendsGenerator>(),
                Component.For<IDataBus>().ImplementedBy<SynchronousInMemoryDataBus>(),
                Component.For<DataProducer>().DependsOn(new { period = int.Parse(WebConfigurationManager.AppSettings["DataGenerationPeriod"], CultureInfo.InvariantCulture) }),
                Component.For<StoringDataConsumer>()
                );

            // Web infrastructure
            container.Register(
                Classes.FromAssemblyContaining<HomeController>()
                .IncludeNonPublicTypes()
                .BasedOn<IController>().LifestyleTransient());
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container.Kernel));
            

            // Signal-R
            Microsoft.AspNet.SignalR.GlobalHost.DependencyResolver.Register(
                typeof(IHubActivator), () => new WindsorHubActivator(container.Kernel));
            container.Register(
                Component.For<DataTicker>(),
                Classes.FromAssemblyContaining<DataTickerHub>().BasedOn<IHub>().LifestyleTransient()
                );
        }
    }
}