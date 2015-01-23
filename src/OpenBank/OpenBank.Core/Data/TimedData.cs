using System;
using System.Diagnostics;

namespace OpenBank.Core.Data
{
    [DebuggerDisplay("{Timestamp}:{Value}")]
    public sealed class TimedData
    {
        public DateTime Timestamp { get; private set; }
        public int Value { get; private set; }

        public TimedData(int value, DateTime timestamp)
        {
            Timestamp = timestamp;
            Value = value;
        }
    }
}