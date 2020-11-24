using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SolarWinds.Logging.Log4Net;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.DataCache.WCF;
using SolarWinds.SharedCommunication.RateLimiter;
using SolarWinds.SharedCommunication.Utils;

namespace SharedMemoryUser
{

    

    class Program
    {
        static async Task Main(string[] args)
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

        static void RateLimiterTest()
        {
            new RateLimiterTestBench().RunTest();
        }

        static void WcfConsumerCacheTest()
        {
            WcfConsumer.Run();
        }
    }
    public class WcfConsumer
    {
        public static ILogger CreateLogger()
        {
            //
            //Logger creation if no DI context is available nor desired:
            //
            {
                var logConfiguration = new Log4NetConfiguration();
                return logConfiguration.GetLoggerProvider().CreateLogger("SdWanLogging");
            }

            // Register everything with Castle Windsor.
            // This should happen once, when your application starts up.
            var container = new WindsorContainer();

            // Register the logger factory.
            container.Register(Component.For(typeof(ILoggerFactory))
                .UsingFactoryMethod((kernel, model, creationContext) =>
                {
                    // Configure log4net, generate a provider, and add it to the factory.
                    var logConfiguration = new Log4NetConfiguration();
                    logConfiguration.Configure("MyApp.exe.config");
                    var provider = logConfiguration.GetLoggerProvider();
                    var factory = new LoggerFactory();
                    factory.AddProvider(provider);
                    return factory;
                })
                .LifestyleSingleton());

            // Register a factory method for creating ILogger<T> from the factory.
            container.Register(Component.For(typeof(ILogger<>))
                .UsingFactoryMethod((kernel, model, creationContext) =>
                {
                    ILoggerFactory loggerFactory = kernel.Resolve<ILoggerFactory>();
                    var classType = creationContext.GenericArguments.First();
                    Type loggerType = typeof(Logger<>).MakeGenericType(classType);
                    object logger = Activator.CreateInstance(loggerType, loggerFactory);
                    return logger;
                })
                .LifestyleSingleton());


            // Register a class that needs a logger.
            //container.Register(Component.For<IMyLibraryClass>().ImplementedBy<MyLibraryClass>());

            //...

            // Resolve the library class that will do some logging. Invoke it.
            //var libraryClass = container.Resolve<IMyLibraryClass>();
            //libraryClass.DoThings();

            var lgr = container.Resolve<ILogger<Program>>();
            return lgr;
        }

        public static async Task Run()
        {
            try
            {
                ILogger logger = CreateLogger();

                DataCacheServiceClientFactory<string> fac =
                    new DataCacheServiceClientFactory<string>(
                        new AsyncSemaphoreFactory(logger));

                var cache = fac.CreateCache("HwH_meraki.com_apikey_orgKey", TimeSpan.FromMinutes(5));

                var res1 = await cache.GetData(() => Task.FromResult((string)"fdfdfdfd"));
                Console.WriteLine("res1:" + res1);

                var res2 = await cache.GetData(() => Task.FromResult((string)"ggfgfgfgfgf"));
                Console.WriteLine("res2:" + res2);

                await Task.Delay(TimeSpan.FromSeconds(2));

                var res3 = await cache.GetData(() => Task.FromResult((string)"123456"));
                Console.WriteLine("res3:" + res3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }

}
