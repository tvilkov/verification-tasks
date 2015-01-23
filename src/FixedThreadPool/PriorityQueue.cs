using System.Collections.Generic;

namespace FixedThreadPool
{
    sealed class PriorityQueue
    {
        private readonly Queue<ITask> m_LowPriorityQueue = new Queue<ITask>();
        private readonly Queue<ITask> m_NormalPriorityQueue = new Queue<ITask>();
        private readonly Queue<ITask> m_HighPriorityQueue = new Queue<ITask>();
        private int m_HighPriorityDequeued;

        public void Enqueue(ITask task, Priority priority)
        {            
            switch (priority)
            {
                case Priority.Low:
                    m_LowPriorityQueue.Enqueue(task);
                    break;
                case Priority.Normal:
                    m_NormalPriorityQueue.Enqueue(task);
                    break;
                case Priority.High:
                    m_HighPriorityQueue.Enqueue(task);
                    break;
            }
        }

        public ITask Dequeue()
        {
            if (Count == 0) return default(ITask);

            //Return LOW priority task if no tasks of HIGH or NORMAL priority
            if (m_HighPriorityQueue.Count == 0 && m_NormalPriorityQueue.Count == 0) return m_LowPriorityQueue.Dequeue();

            //Return NORMAL priority task for every 3 with HIGH priority or in case there are no tasks with HIGH priority
            if (m_HighPriorityQueue.Count == 0 || (m_HighPriorityDequeued >= 3 && m_NormalPriorityQueue.Count != 0) )        
            {
                m_HighPriorityDequeued = 0;
                return m_NormalPriorityQueue.Dequeue();
            }

            //Otherwise return with HIGH priority
            ++m_HighPriorityDequeued;

            return m_HighPriorityQueue.Dequeue();
        }

        public int Count
        {
            get { return m_LowPriorityQueue.Count + m_NormalPriorityQueue.Count + m_HighPriorityQueue.Count; }
        }
    }
}