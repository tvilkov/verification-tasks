using System;
using System.Runtime.InteropServices;

namespace Kontur.ImageTransformer.Util
{
    // ReSharper disable once InconsistentNaming
    internal static class WinAPI
    {
        [DllImport("psapi.dll")]
        public static extern int EmptyWorkingSet(IntPtr hwProc);

        [DllImport("httpapi.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern uint HttpSetRequestQueueProperty(
            CriticalHandle requestQueueHandle,
            HTTP_SERVER_PROPERTY serverProperty,
            IntPtr pPropertyInfo,
            uint propertyInfoLength,
            uint reserved,
            IntPtr pReserved);
    }

    // ReSharper disable once InconsistentNaming
    internal enum HTTP_SERVER_PROPERTY
    {
        HttpServerAuthenticationProperty,
        HttpServerLoggingProperty,
        HttpServerQosProperty,
        HttpServerTimeoutsProperty,
        HttpServerQueueLengthProperty,
        HttpServerStateProperty,
        HttpServer503VerbosityProperty,
        HttpServerBindingProperty,
        HttpServerExtendedAuthenticationProperty,
        HttpServerListenEndpointProperty,
        HttpServerChannelBindProperty,
        HttpServerProtectionLevelProperty,
    }
}