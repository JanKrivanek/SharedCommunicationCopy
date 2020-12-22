using System;
using SolarWinds.SharedCommunication.DataCache.WCF;
using SolarWinds.SharedCommunication.Utils;

namespace SharedMemoryProvider
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            PollerDataCacheService pdc = new PollerDataCacheService(new PlatformDateTime());
            pdc.Start();
            Console.ReadKey();
        }
    }
}
