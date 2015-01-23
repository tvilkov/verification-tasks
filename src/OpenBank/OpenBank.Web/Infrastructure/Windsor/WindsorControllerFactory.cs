using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;

namespace OpenBank.Web.Infrastructure.Windsor
{
    internal sealed class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel m_Kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            m_Kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            m_Kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            return (IController)m_Kernel.Resolve(controllerType);
        }
    }
}
