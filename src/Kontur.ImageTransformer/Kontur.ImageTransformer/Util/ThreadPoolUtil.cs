using System;
using System.Threading;

namespace Kontur.ImageTransformer.Util
{
    internal static class ThreadPoolUtil
    {
        public static readonly int MaxThreads = 32767;

        public static void Configure(int expandFactor = 64)
        {
            if (expandFactor <= 0) throw new ArgumentOutOfRangeException(nameof(expandFactor));

            var minThreads = Math.Min(Environment.ProcessorCount * expandFactor, MaxThreads);

            ThreadPool.SetMinThreads(minThreads, minThreads);
            ThreadPool.SetMaxThreads(MaxThreads, MaxThreads);
        }
    }
}