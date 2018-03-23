using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Util
{
    internal static class Empty
    {
        public static readonly Task Task = Task.FromResult(true);
    }
}