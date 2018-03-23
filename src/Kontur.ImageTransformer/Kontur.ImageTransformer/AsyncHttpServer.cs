using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Drawing;
using Kontur.ImageTransformer.Handlers;
using Kontur.ImageTransformer.Monitoring;
using Kontur.ImageTransformer.Util;

namespace Kontur.ImageTransformer
{
    internal class AsyncHttpServer : IDisposable
    {
        public AsyncHttpServer()
        {
            m_Listener = new HttpListener();
            //m_Handler = new ImageFilterHandler(new FiltersRegistry(),  new BitmapImageFactory());
            m_Handler = new ImageTransformHandler(new BitmapImageFactory());
            m_RequestHandlingTimeout = TimeSpan.FromMilliseconds(500); // Complete processing of any request witin 0.5 sec
            m_RequestRateLimiter = new RequestRateLimiter(Constants.MAX_RPS);
        }

        public void Start(string prefix)
        {
            lock (m_Listener)
            {
                if (m_IsRunning) return;

                m_Listener.Prefixes.Clear();
                m_Listener.Prefixes.Add(prefix);
                m_Listener.IgnoreWriteExceptions = true;
                m_Listener.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(1);
                m_Listener.TimeoutManager.EntityBody = TimeSpan.FromSeconds(1);
                m_Listener.TimeoutManager.DrainEntityBody = TimeSpan.FromSeconds(1);
                m_Listener.TimeoutManager.MinSendBytesPerSecond = uint.MaxValue;

                m_Listener.Start();

                try
                {
                    m_Listener.SetRequestQueueLength(10);
                }
                catch (Exception)
                {
                    // Ignore
                }

                m_RequestRateLimiter.Start();

                m_ListenerThread = new Thread(listen)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Highest,
                    CurrentCulture = CultureInfo.InvariantCulture,
                };
                m_ListenerThread.Start();

                m_IsRunning = true;
            }
        }

        public void Stop()
        {
            lock (m_Listener)
            {
                if (!m_IsRunning) return;

                m_Listener.Stop();

                m_ListenerThread.Abort();
                m_ListenerThread.Join();

                m_RequestRateLimiter.Stop();

                m_IsRunning = false;
            }
        }

        public void Dispose()
        {
            if (m_Disposed) return;

            m_Disposed = true;

            Stop();

            m_RequestRateLimiter.Dispose();

            m_Listener.Close();
        }

        private void listen()
        {
            while (true)
                try
                {
                    if (m_Listener.IsListening)
                    {
                        var listenerContext = m_Listener.GetContext();

                        listenerContext.Response.Headers.Set("Server", "");
                        listenerContext.Response.Headers.Set("Date", "");

                        if (m_RequestRateLimiter.AcceptRequest())
                        {
                            Task.Factory.StartNew(async () => await handleContextAsync(listenerContext).ConfigureAwait(false));
                        }
                        else
                        {
                            listenerContext.SendServerBusy();
                        }
                    }
                    else
                    {
                        Thread.Sleep(0);
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception)
                {
                    // TODO: log errors
                }
        }

        private async Task handleContextAsync(HttpListenerContext listenerContext)
        {
            try
            {
                using (var cts = new CancellationTokenSource())
                {
                    cts.CancelAfter(m_RequestHandlingTimeout);
                    bool handled;
                    try
                    {
                        handled = await m_Handler.Handle(listenerContext, cts.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        listenerContext.SendServerBusy();
                        return;
                    }

                    if (!handled) listenerContext.SendBadRequest("The route can't be handled");
                }
            }
            catch (Exception)
            {
                // TODO: log                
                listenerContext.SendInternalServerError();
            }
        }

        private readonly HttpListener m_Listener;
        private Thread m_ListenerThread;
        private readonly IRequestHandler m_Handler;
        private readonly TimeSpan m_RequestHandlingTimeout;
        private bool m_Disposed;
        private volatile bool m_IsRunning;
        private readonly RequestRateLimiter m_RequestRateLimiter;
    }
}