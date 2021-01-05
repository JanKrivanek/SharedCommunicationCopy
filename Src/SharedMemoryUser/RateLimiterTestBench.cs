using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SolarWinds.SharedCommunication.Utils;

namespace SolarWinds.SharedCommunication.RateLimiter
{

    public class RateLimiterTestBench
    {
        public void RunTest()
        {
            string apiBaseAddress = "https://meraki123/api/v2";
            string apiKey = "15151v2cv1"; //+org id
            string orgId = null;

            string id = new SynchronizationIdentifiersProvider().GetSynchronizationIdentifier(apiBaseAddress, apiKey,
                orgId);

            ILogger logger = NullLogger.Instance;

            CrossProcessRateLimiterFactory f = new CrossProcessRateLimiterFactory(new PlatformDateTime(),
                new KernelObjectsPrivilegesChecker(logger));

            int workersCount = 8;
            Parallel.For(1, workersCount + 1, workerId =>
            {
                var limiter = f.OpenOrCreate(id, TimeSpan.FromMilliseconds(1000), 5);
                for (int callId = 0; callId < 20; callId++)
                {
                    Console.WriteLine($"{DateTime.UtcNow.ToString("dd-MM-ss.fffffff")} worker [{workerId}] starting call {callId}");
                    bool canRun = limiter.BlockTillNextFreeSlot(TimeSpan.FromSeconds(10));
                    Console.WriteLine($"{DateTime.UtcNow.ToString("dd-MM-ss.fffffff")} worker [{workerId}] finished call {callId}. Success: {canRun}");
                }
            });
        }
    }
}

