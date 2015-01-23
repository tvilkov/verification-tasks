using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenBank.Core.Extensions;

namespace OpenBank.Core.Transport
{
    public sealed class SynchronousInMemoryDataBus : IDataBus, IDisposable
    {
        private readonly ICollection<Delegate> m_Subscriptions = new Collection<Delegate>();

        public void Publish<T>(T data)
        {
            Delegate[] snapshot;
            lock (m_Subscriptions)
            {
                snapshot = m_Subscriptions.ToArray();
            }
            snapshot.Apply<Action<T>>(s => s(data));
        }

        public IDisposable Subscribe<T>(Action<T> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            lock (m_Subscriptions)
            {
                if (!m_Subscriptions.Contains(handler))
                    m_Subscriptions.Add(handler);
            }

            return ActionDisposable.Create(() =>
                {
                    lock (m_Subscriptions)
                    {
                        m_Subscriptions.Remove(handler);
                    }
                });

        }

        public void Dispose()
        {
            m_Subscriptions.Clear();
        }
    }
}
