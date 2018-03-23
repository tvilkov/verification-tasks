using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Monitoring
{
    internal abstract class PeriodicJob : IDisposable
    {
        private readonly TimeSpan m_Period;
        private Task m_Worker;
        private CancellationTokenSource m_CancellationTokenSource;
        private readonly object m_SyncLock = new object();

        protected PeriodicJob(TimeSpan period)
        {
            if (TimeSpan.Zero == period) throw new ArgumentOutOfRangeException(nameof(period));
            m_Period = period;
        }

        public void Start()
        {
            if (m_CancellationTokenSource != null) return;
			
            lock (m_SyncLock)
            {
                if (m_CancellationTokenSource != null) return;
                m_CancellationTokenSource = new CancellationTokenSource();
                m_Worker = Task.Factory.StartNew(async () =>
                {
                    while (!m_CancellationTokenSource.IsCancellationRequested)
                    {
                        await WorkingRoutine(m_CancellationTokenSource.Token);
                        await Task.Delay(m_Period, m_CancellationTokenSource.Token);
                    }
                }, TaskCreationOptions.LongRunning);
            }
        }

        public void Stop()
        {
            if (m_CancellationTokenSource == null) return;

            lock (m_SyncLock)
            {
                if (m_CancellationTokenSource == null) return;
                m_CancellationTokenSource.Cancel();
                m_Worker.Wait();
                m_Worker.Dispose();
                m_CancellationTokenSource.Dispose();
                m_Worker = null;
                m_CancellationTokenSource = null;
            }
        }

        public virtual void Dispose()
        {
            Stop();
        }

        protected abstract Task WorkingRoutine(CancellationToken cancellationToken);
    }
}