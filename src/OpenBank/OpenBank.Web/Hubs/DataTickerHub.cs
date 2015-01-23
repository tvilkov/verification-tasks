using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using OpenBank.Core.Data;
using OpenBank.Core.Extensions;
using OpenBank.Core.Persistence;
using OpenBank.Core.Transport;

namespace OpenBank.Web.Hubs
{
    [HubName("dataTicker")]
    public class DataTickerHub : Hub
    {
        private readonly DataTicker m_DataTicker;

        public DataTickerHub(DataTicker dataTicker)
        {
            if (dataTicker == null) throw new ArgumentNullException("dataTicker");
            m_DataTicker = dataTicker;
        }

        public IEnumerable<object> GetLastHourData()
        {
            return m_DataTicker.GetLastHourData();
        }
    }

    /// <summary>
    /// This class holds all the logic regarding propagating data updates to clients (web-browsers).
    /// We can't put this logic directly to the SignalR's hub class which is recreated on every client request.
    /// </summary>
    public sealed class DataTicker : IDisposable
    {
        private readonly IDataStorage m_DataStorage;
        private IDisposable m_Subscription;
        private IHubContext m_HubContext;

        public DataTicker(IDataStorage dataStorage, IDataBus dataBus)
        {
            if (dataStorage == null) throw new ArgumentNullException("dataStorage");
            if (dataBus == null) throw new ArgumentNullException("dataBus");
            m_DataStorage = dataStorage;
            m_Subscription = dataBus.Subscribe<TimedData>(notifyClients);
            m_HubContext = GlobalHost.ConnectionManager.GetHubContext<DataTickerHub>();
        }

        public IEnumerable<object> GetLastHourData()
        {
            var end = DateTime.Now;
            var start = end.AddHours(-1);
            var data = m_DataStorage.LoadAll(start, end).Select(mapToJsModel).ToArray();
            return data;
        }

        private void notifyClients(TimedData data)
        {
            if (data == null) throw new ArgumentNullException("data");
            m_HubContext.Clients.All.onNewData(mapToJsModel(data));
        }

        private static object mapToJsModel(TimedData data)
        {
            if (data == null) throw new ArgumentNullException("data");
            return new
                {
                    time = data.Timestamp.ToJsTimestamp(),
                    val = data.Value
                };
        }

        public void Dispose()
        {
            if (m_Subscription != null)
            {
                m_Subscription.Dispose();
                m_Subscription = null;
                m_HubContext = null;
            }
        }
    }
}