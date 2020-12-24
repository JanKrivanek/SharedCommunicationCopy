using System;
using System.Threading.Tasks;

namespace SharedMemoryUser
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("WcfConsumer test will run on keypress (do not forget to start the server endpoint via SharedMemoryProvider project)");
            Console.ReadKey();

            WcfConsumerCacheTest();

            Console.WriteLine("WcfConsumer test done");
            Console.WriteLine("RateLimiter test will run on keypress");
            Console.ReadKey();

            Console.WriteLine("RateLimiter test done");
            Console.WriteLine("Will exit on keypress");
            Console.ReadKey();

            return;
        }

        private static void WcfConsumerCacheTest()
        {
            WcfConsumer.Run();
        }
    }
}
