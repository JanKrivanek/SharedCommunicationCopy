using System;

namespace SolarWinds.SharedCommunication.Contracts.RateLimiter
{
    /// <summary>
    /// interface for cross process rate limiter factory
    /// </summary>
    public interface ICrossProcessRateLimiterFactory
    {
        /// <summary>
        /// opens or creates a rate limiter
        /// </summary>
        /// <param name="identifier"> identifies </param>
        /// <param name="measureTime"> time parameter for span ticks in limiter</param>
        /// <param name="maxOccurencesPerTime"> limiter capacity</param>
        /// <returns></returns>
        IRateLimiter OpenOrCreate(string identifier, TimeSpan measureTime, int maxOccurencesPerTime);
    }
}