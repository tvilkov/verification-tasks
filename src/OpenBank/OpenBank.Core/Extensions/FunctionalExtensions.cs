using System;

namespace OpenBank.Core.Extensions
{
    public static class FunctionalExtensions
    {
        public static IDisposable Periodic(this Action action, TimeSpan period)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (period == null) throw new ArgumentNullException("period");
            if (period == TimeSpan.Zero) throw new ArgumentException("period must specify positive number of seconds", "period");

            System.Threading.TimerCallback cb = _ => action();
            var timer = new System.Threading.Timer(cb, null, TimeSpan.Zero, period);
            return ActionDisposable.Create(timer.Dispose);
        }
    }
}