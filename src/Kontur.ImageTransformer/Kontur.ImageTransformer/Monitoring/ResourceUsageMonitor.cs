using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Util;

namespace Kontur.ImageTransformer.Monitoring
{
    internal class ResourceUsageMonitor : PeriodicJob
    {
        private const int AVAILABLE_RAM_LIMIT_MB = 512; // Always leave at least 512Mb of 8Gb available on test machine
        private const int BYTES_IN_MB = 1024 * 1024;
        private readonly PerformanceCounter m_CpuCounter;
        private readonly PerformanceCounter m_RamCounter;
        private readonly IntPtr m_CurrentProcessHandle;
        private int m_CpuLoad;
        private int m_RamAvailable;

        public ResourceUsageMonitor() : base(TimeSpan.FromSeconds(3))
        {
            m_CpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            m_RamCounter = new PerformanceCounter("Memory", "Available Bytes");
            m_CurrentProcessHandle = Process.GetCurrentProcess().Handle;
        }

        protected override async Task WorkingRoutine(CancellationToken cancellationToken)
        {
            var cpuLoad = (int) Math.Floor(m_CpuCounter.NextValue());
            var memoryAvailable = (int) Math.Floor(m_RamCounter.NextValue() / (1024 * 1024));

            if (memoryAvailable < AVAILABLE_RAM_LIMIT_MB)
            {
                Console.WriteLine($"Low memory {memoryAvailable}Mb, running GC and shrinking working set");
                GC.Collect();
                WinAPI.EmptyWorkingSet(m_CurrentProcessHandle);
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken); // Wait a while to give the counters time to update
                memoryAvailable = (int) Math.Floor(m_RamCounter.NextValue() / BYTES_IN_MB);
            }

            Volatile.Write(ref m_CpuLoad, cpuLoad);
            Volatile.Write(ref m_RamAvailable, memoryAvailable);
        }

        public int CpuLoad => Volatile.Read(ref m_CpuLoad); // Percents

        public int RamAvailable => Volatile.Read(ref m_RamAvailable); // Mb

        public override void Dispose()
        {
            base.Dispose();
            m_CpuCounter.Dispose();
            m_RamCounter.Dispose();
        }
    }
}