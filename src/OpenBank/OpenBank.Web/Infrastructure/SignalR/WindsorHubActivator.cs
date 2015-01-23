using Castle.MicroKernel;
using Microsoft.AspNet.SignalR.Hubs;

namespace OpenBank.Web.Infrastructure.SignalR
{
    internal sealed class WindsorHubActivator : IHubActivator
    {
        private readonly IKernel m_Kernel;

        public WindsorHubActivator(IKernel kernel)
        {
            m_Kernel = kernel;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return m_Kernel.HasComponent(descriptor.HubType) ? m_Kernel.Resolve(descriptor.HubType) as IHub : null;
        }
    }
}