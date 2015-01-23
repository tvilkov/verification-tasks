using System;
using OpenBank.Core.Data;
using OpenBank.Core.Persistence;
using OpenBank.Core.Transport;

namespace OpenBank.Core
{
    public sealed class StoringDataConsumer : IDisposable
    {
        private readonly IDataBus m_DataBus;
        private readonly IDataStorage m_DataStorage;
        private IDisposable m_Subscription;

        public StoringDataConsumer(IDataBus dataBus, IDataStorage dataStorage)
        {
            if (dataBus == null) throw new ArgumentNullException("dataBus");
            if (dataStorage == null) throw new ArgumentNullException("dataStorage");
            m_DataBus = dataBus;
            m_DataStorage = dataStorage;
        }

        public void Start()
        {
            if (m_Subscription != null) throw new InvalidOperationException("Already started");
            m_Subscription = m_DataBus.Subscribe<TimedData>(cb => m_DataStorage.Save(cb));
        }

        public void Stop()
        {
            if (m_Subscription != null)
            {
                m_Subscription.Dispose();
                m_Subscription = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}