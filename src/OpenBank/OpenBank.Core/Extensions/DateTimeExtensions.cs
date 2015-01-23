using System;

namespace OpenBank.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static DateTime m_EraStart = new DateTime(1970, 1, 1);

        public static long ToJsTimestamp(this DateTime date)
        {
            var span = new TimeSpan(m_EraStart.Ticks);
            var time = date.Subtract(span);
            return time.Ticks / 10000;
        }
    }
}