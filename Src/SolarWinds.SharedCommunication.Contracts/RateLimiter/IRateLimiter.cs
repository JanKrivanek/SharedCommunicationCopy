using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolarWinds.SharedCommunication.Contracts.RateLimiter
{
    /// <summary>
    /// interface for rate limiter
    /// </summary>
    public interface IRateLimiter
    {
        /// <summary>
        /// task for waiting until there is a next free slot
        /// </summary>
        /// <param name="maxAcceptableWaitingTime"> limit of time waiting </param>
        /// <param name="cancellationToken"> cancellation token </param>
        /// <returns></returns>
        Task<bool> WaitTillNextFreeSlotAsync(TimeSpan maxAcceptableWaitingTime, CancellationToken cancellationToken = default);

        /// <summary>
        /// tstates if thread should sleep until there is next free slot
        /// </summary>
        /// <param name="maxAcceptableWaitingTime"> limit of time to wait </param>
        /// <returns></returns>
        bool BlockTillNextFreeSlot(TimeSpan maxAcceptableWaitingTime);

    }
}