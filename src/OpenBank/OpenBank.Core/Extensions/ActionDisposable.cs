using System;

namespace OpenBank.Core.Extensions
{
    public sealed class ActionDisposable : IDisposable
    {
        private Action m_OnDispose;

        private ActionDisposable(Action onDispose)
        {
            if (onDispose == null) throw new ArgumentNullException("onDispose");
            m_OnDispose = onDispose;
        }

        public void Dispose()
        {
            if (m_OnDispose != null)
            {
                m_OnDispose();
                m_OnDispose = null;
            }
        }

        public static IDisposable Create(Action disposer)
        {
            if (disposer == null) throw new ArgumentNullException("disposer");
            return new ActionDisposable(disposer);
        }
    }
}