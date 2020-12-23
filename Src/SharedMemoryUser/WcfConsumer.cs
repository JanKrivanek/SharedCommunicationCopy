using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Extensions.Logging;
using SolarWinds.Logging.Log4Net;
using SolarWinds.SharedCommunication.DataCache.WCF;
using SolarWinds.SharedCommunication.Utils;

namespace SharedMemoryUser
{
    /// <summary>
    /// a testing class for WCF consumer
    /// </summary>
    public class WcfConsumer
    {
        /// <summary>
        /// creates logger for specfied DI context
        /// </summary>
        /// <param name="useDi"> shows whether DI context is present</param>
        /// <returns></returns>
        public static ILogger CreateLogger(bool useDi)
        {
            //
            //Logger creation if no DI context is available nor desired:
            //
            if (!useDi)
            {
                var logConfiguration = new Log4NetConfiguration();
                return logConfiguration.GetLoggerProvider().CreateLogger("SdWanLogging");
            }

            //Logger creation with Castle.Winsdor DI context
            else
            {
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

                var lgr = container.Resolve<ILogger<Program>>();
                return lgr;
            }
        }

        public static async Task Run()
        {
            try
            {
                ILogger logger = CreateLogger(false);

                DataCacheServiceClientFactory<string> fac =
                    new DataCacheServiceClientFactory<string>(
                        new AsyncSemaphoreFactory(logger));

                var cache = fac.CreateCache("HwH_meraki.com_apikey_orgKey", TimeSpan.FromMinutes(5));

                var res1 = await cache.GetDataAsync(() => Task.FromResult((string)"fdfdfdfd"));
                Console.WriteLine("res1:" + res1);

                var res2 = await cache.GetDataAsync(() => Task.FromResult((string)"ggfgfgfgfgf"));
                Console.WriteLine("res2:" + res2);

                await Task.Delay(TimeSpan.FromSeconds(2));

                var res3 = await cache.GetDataAsync(() => Task.FromResult((string)"123456"));
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
