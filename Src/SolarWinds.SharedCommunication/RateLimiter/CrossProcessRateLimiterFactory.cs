using System;
using SolarWinds.SharedCommunication.Contracts.RateLimiter;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.RateLimiter
{
    /// <summary>
    /// a class for cross process rate limiter factory
    /// </summary>
    public class CrossProcessRateLimiterFactory: ICrossProcessRateLimiterFactory
    {
        private readonly IDateTime dateTime;
        private readonly IKernelObjectsPrivilegesChecker privilegesChecker;

        public CrossProcessRateLimiterFactory(IDateTime dateTime, IKernelObjectsPrivilegesChecker privilegesChecker)
        {
            this.dateTime = dateTime;
            this.privilegesChecker = privilegesChecker;
        }

        /// <summary>
        /// opens or creats a rate limiter shared memory accessor
        /// </summary>
        /// <param name="identifier"> id of the accessor </param>
        /// <param name="measureTime"> time parameter to get span ticks for the accessor </param>
        /// <param name="maxOccurencesPerTime"> capacity of the accessor</param>
        /// <returns></returns>
        public IRateLimiter OpenOrCreate(string identifier, TimeSpan measureTime, int maxOccurencesPerTime)
        {
            RateLimiterSharedMemoryAccessor accesor =
                new RateLimiterSharedMemoryAccessor(identifier, maxOccurencesPerTime, measureTime.Ticks, privilegesChecker);
            return new RingMemoryBufferRateLimiter(accesor, dateTime);
        }
    }
}
