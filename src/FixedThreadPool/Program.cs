using System;
using System.Diagnostics;
using System.Threading;

namespace FixedThreadPool
{
    class Program
    {
// ReSharper disable InconsistentNaming
        static void Main(string[] args)
// ReSharper restore InconsistentNaming
        {
            var threadPool = new FixedThreadPool(2);
        		Thread.Sleep( 5000 );
						threadPool.Execute( new SleepyTask( "Task 0 (HIGH)", 5 ), Priority.High );
						threadPool.Execute( new SleepyTask( "Task 1 (HIGH)", 5 ), Priority.High );
						threadPool.Execute( new SleepyTask( "Task 2 (HIGH)", 5 ), Priority.High );
						threadPool.Execute( new SleepyTask( "Task 3 (LOW)", 3 ), Priority.Low );
						threadPool.Execute( new SleepyTask( "Task 4 (LOW)", 3 ), Priority.Low );
						threadPool.Execute( new SleepyTask( "Task 5 (NORMAL)", 3 ), Priority.Normal );
						threadPool.Execute( new SleepyTask( "Task 6 (NORMAL)", 3 ), Priority.Normal );
						threadPool.Execute( new SleepyTask( "Task 7 (NORMAL)", 3 ), Priority.Normal );
						threadPool.Execute( new SleepyTask( "Task 8 (NORMAL)", 3 ), Priority.Normal );
						threadPool.Execute( new SleepyTask( "Task 9 (HIGH)", 3 ), Priority.High );
						threadPool.Execute( new SleepyTask( "Task 10 (HIGH)", 3 ), Priority.High );
						threadPool.Execute( new SleepyTask( "Task 11 (HIGH)", 3 ), Priority.High );
						threadPool.Execute( new SleepyTask( "Task 12 (HIGH)", 3 ), Priority.High );
						threadPool.Execute( new SleepyTask( "Task 13 (LOW)", 3 ), Priority.Low );
            threadPool.Stop();
        }
    }

    [DebuggerDisplay("Task {m_Name}")]
    sealed class SleepyTask : ITask
    {
        private readonly string m_Name;
        private readonly int m_Delay;

        public SleepyTask(string name, int delay)
        {
            if (name == null) throw new ArgumentNullException("name");

            m_Name = name;
            m_Delay = delay;
        }

        public void Execute()
        {
            Console.WriteLine(string.Format("Executing task {0} on thread {1}", m_Name, Thread.CurrentThread.Name));
            Thread.Sleep(1000*m_Delay);
        }
    }
}
