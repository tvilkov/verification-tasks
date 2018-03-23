using System;
using Kontur.ImageTransformer.Monitoring;
using Kontur.ImageTransformer.Util;

namespace Kontur.ImageTransformer
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            ThreadPoolUtil.Configure(64);

            using (var resourceUsageMonitor = new ResourceUsageMonitor())
            using (var server = new AsyncHttpServer())
            {
                resourceUsageMonitor.Start();
                server.Start("http://+:8080/");
                Console.WriteLine("Server started. Press any key to stop...");
                Console.ReadKey(true);
                server.Stop();
                resourceUsageMonitor.Stop();
            }
        }
    }
}