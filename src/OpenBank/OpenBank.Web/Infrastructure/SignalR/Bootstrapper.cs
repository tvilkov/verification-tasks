using System.ComponentModel;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OpenBank.Web.Infrastructure.SignalR.Bootstrapper))]

namespace OpenBank.Web.Infrastructure.SignalR
{
    public sealed class Bootstrapper
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    EnableJavaScriptProxies = true
                };
            app.MapSignalR(config);
        }
    }
}