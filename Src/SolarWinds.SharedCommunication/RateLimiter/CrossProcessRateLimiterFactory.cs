using System;
using SolarWinds.SharedCommunication.Contracts.RateLimiter;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.RateLimiter
{
    /// <summary>.
    /// A class for cross process rate limiter factory
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
        /// Opens or creats a rate limiter shared memory accessor based on string identifier, time span and accessor capacity.
        /// </summary>
        /// <param name="identifier">Id of the accessor.</param>
        /// <param name="measureTime">Time parameter to get span ticks for the accessor.</param>
        /// <param name="maxOccurencesPerTime">Capacity of the accessor.</param>
        public IRateLimiter OpenOrCreate(string identifier, TimeSpan measureTime, int maxOccurencesPerTime)
        {
            RateLimiterSharedMemoryAccessor accesor =
                new RateLimiterSharedMemoryAccessor(identifier, maxOccurencesPerTime, measureTime.Ticks, privilegesChecker);
            return new RingMemoryBufferRateLimiter(accesor, dateTime);
        }
    }
}
