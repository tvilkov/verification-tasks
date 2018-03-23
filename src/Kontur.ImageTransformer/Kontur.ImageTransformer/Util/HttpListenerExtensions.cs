using System;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Kontur.ImageTransformer.Util
{
    internal static class HttpListenerExtensions
    {
        public static HttpListenerContext SendResponse(this HttpListenerContext context, int statusCode, string statusDescription = null)
        {
            context.Response.StatusCode = statusCode;
            if (statusDescription != null)
            {
                context.Response.StatusDescription = statusDescription;
            }

            context.Response.Close();
            return context;
        }

        public static HttpListenerContext SendBadRequest(this HttpListenerContext context, string reason)
        {
            return SendResponse(context, (int) HttpStatusCode.BadRequest, reason);
        }

        public static HttpListenerContext SendServerBusy(this HttpListenerContext context)
        {
            return SendResponse(context, 429); // To many requests
        }

        public static HttpListenerContext SendNoContent(this HttpListenerContext context)
        {
            return SendResponse(context, (int) HttpStatusCode.NoContent);
        }

        public static HttpListenerContext SendInternalServerError(this HttpListenerContext context)
        {
            return SendResponse(context, (int) HttpStatusCode.InternalServerError);
        }

        public static unsafe void SetRequestQueueLength(this HttpListener listener, long len)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            if (len <= 0) throw new ArgumentOutOfRangeException(nameof(len));

            // HttpListener stores request's queue handle in internal property.
            // Reflection is the only way to get there.
            var requestQueueHandleProperty = typeof(HttpListener).GetProperty("RequestQueueHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            if (requestQueueHandleProperty == null)
            {
                throw new NotImplementedException("Unsupported API");
            }

            var requestQueueHandle = (CriticalHandle) requestQueueHandleProperty.GetValue(listener);

            // Listener is not started yet, consider this OK
            if (requestQueueHandle == null)
            {
                return;
            }

            var result = WinAPI.HttpSetRequestQueueProperty(requestQueueHandle,
                HTTP_SERVER_PROPERTY.HttpServerQueueLengthProperty,
                new IntPtr(&len),
                (uint) Marshal.SizeOf(len),
                0,
                IntPtr.Zero);

            if (result != 0)
            {
                throw new HttpListenerException((int) result);
            }
        }
    }
}