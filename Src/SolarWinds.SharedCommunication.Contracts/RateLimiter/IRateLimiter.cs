using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolarWinds.SharedCommunication.Contracts.RateLimiter
{
    /// <summary>
    /// An interface for rate limiter.
    /// </summary>
    public interface IRateLimiter
    {
        /// <summary>
        /// A task for waiting until there is a next free slot based on time limit and cancellation token.
        /// </summary>
        /// <param name="maxAcceptableWaitingTime">Limit of time waiting.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<bool> WaitTillNextFreeSlotAsync(TimeSpan maxAcceptableWaitingTime, CancellationToken cancellationToken = default);

        /// <summary>
        /// States if thread should sleep until there is next free slot based on time limit.
        /// </summary>
        /// <param name="maxAcceptableWaitingTime"> limit of time to wait.</param>
        bool BlockTillNextFreeSlot(TimeSpan maxAcceptableWaitingTime);

    }
}