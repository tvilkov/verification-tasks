using System;
using System.Collections.Generic;
using System.Threading;

namespace FixedThreadPool
{
    public sealed class FixedThreadPool : IFixedThreadPool
    {
        readonly object m_Locker = new object();
        private readonly List<Thread> m_Threads = new List<Thread>();
        private readonly PriorityQueue m_Tasks = new PriorityQueue();
        private volatile bool m_Stopped;

        public FixedThreadPool(int numberOfThreads)
        {
            if (numberOfThreads <= 0) throw new ArgumentException("numberOfThreads must be positive integer value");

            createThreads(numberOfThreads);
        }

        public bool Execute(ITask task, Priority priority)
        {
            if (task == null) throw new ArgumentNullException("task");

            if (m_Stopped) return false;

            lock (m_Locker)
            {
                m_Tasks.Enqueue(task, priority);
                Monitor.PulseAll(m_Locker); //Let the other threads go
            }

            return true;
        }

				public void Stop()
				{
					m_Stopped = true;

					//FIX: 
					//	Pulse to working threads has been forgotten potentially causing application hanging on Stop() call
					lock ( m_Locker )
					{
						m_Threads.ForEach( t => m_Tasks.Enqueue( null, Priority.Low ) );
						Monitor.PulseAll( m_Locker );
					}

					m_Threads.ForEach( thread => thread.Join() );
				}

        private void createThreads(int numberOfThreads)
        {
            for (int i = 0; i < numberOfThreads; ++i)
            {
                var thread = new Thread(workingRoutine) { IsBackground = true, Name = "FixedThreadPool thread #" + i };
                m_Threads.Add(thread);
                thread.Start();
            }
        }

        private void workingRoutine()
        {
            while (true)
            {
                ITask task;
                lock (m_Locker)
                {
                    while (m_Tasks.Count == 0) Monitor.Wait(m_Locker); //Wait until something is in the queue
                    task = m_Tasks.Dequeue();                    
                }
                if (task == null) break;
                task.Execute();
            }
        }
    }
}