using System;
using System.Diagnostics;
using OpenBank.Core.Data;
using OpenBank.Core.Extensions;
using OpenBank.Core.Transport;

namespace OpenBank.Core
{
    public sealed class DataProducer : IDisposable
    {
        private readonly IDataBus m_DataBus;
        private readonly IDataGenerator m_DataGenerator;
        private readonly int m_Period;
        private IDisposable m_Repeater;

        public DataProducer(IDataBus dataBus, IDataGenerator dataGenerator, int period = 3)
        {
            if (dataBus == null) throw new ArgumentNullException("dataBus");
            if (dataGenerator == null) throw new ArgumentNullException("dataGenerator");
            if (period <= 0) throw new ArgumentException("period must be a positive value in seconds", "period");
            m_DataBus = dataBus;
            m_DataGenerator = dataGenerator;
            m_Period = period;
        }

        public void Start()
        {
            if (m_Repeater != null) throw new InvalidOperationException("Already started");

            Action @do = produceNextValue;
            m_Repeater = @do.Periodic(TimeSpan.FromSeconds(m_Period));
        }

        public void Stop()
        {
            if (m_Repeater != null)
            {
                m_Repeater.Dispose();
                m_Repeater = null;
            }
        }

        void produceNextValue()
        {
            var nextVal = m_DataGenerator.GetNextValue();
            var nextData = new TimedData(nextVal, DateTime.Now);
            Debug.WriteLine("Data produced. Timestamp={0}, Value={1}", nextData.Timestamp, nextData.Value);
            m_DataBus.Publish(nextData);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}