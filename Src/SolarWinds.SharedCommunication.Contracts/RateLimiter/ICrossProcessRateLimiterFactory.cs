using System;

namespace SolarWinds.SharedCommunication.Contracts.RateLimiter
{
    public interface ICrossProcessRateLimiterFactory
    {
        IRateLimiter OpenOrCreate(string identifier, TimeSpan measureTime, int maxOccurencesPerTime);
    }
}