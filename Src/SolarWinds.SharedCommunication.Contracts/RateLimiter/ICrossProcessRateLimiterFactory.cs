using System;

namespace SolarWinds.SharedCommunication.Contracts.RateLimiter
{
    /// <summary>
    /// An interface for cross process rate limiter factory.
    /// </summary>
    public interface ICrossProcessRateLimiterFactory
    {
        /// <summary>
        /// Opens or creates a rate limiter based on identifier string, time span and limiter capacity.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="measureTime">Time parameter for span ticks in limiter.</param>
        /// <param name="maxOccurencesPerTime">Limiter capacity.</param>
        /// <returns>The opened limiter.</returns>
        IRateLimiter OpenOrCreate(string identifier, TimeSpan measureTime, int maxOccurencesPerTime);
    }
}