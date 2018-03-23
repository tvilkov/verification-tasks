using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Handlers
{
    internal interface IRequestHandler
    {
        Task<bool> Handle(HttpListenerContext context, CancellationToken cancellationToken);
    }
}