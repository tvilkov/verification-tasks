using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Util;

namespace Kontur.ImageTransformer.Monitoring
{
    internal class RequestRateLimiter : PeriodicJob
    {
        private readonly int m_MaxRequestsPerSecond;
        private int m_IncomingRequestsCount;

        public RequestRateLimiter(int maxRequestsPerSecond) : base(TimeSpan.FromSeconds(1))
        {
            m_MaxRequestsPerSecond = maxRequestsPerSecond;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AcceptRequest()
        {
            return Interlocked.Increment(ref m_IncomingRequestsCount) <= m_MaxRequestsPerSecond;
        }

        protected override Task WorkingRoutine(CancellationToken cancellationToken)
        {
            Interlocked.Exchange(ref m_IncomingRequestsCount, 0);
            return Empty.Task;
        }
    }
}