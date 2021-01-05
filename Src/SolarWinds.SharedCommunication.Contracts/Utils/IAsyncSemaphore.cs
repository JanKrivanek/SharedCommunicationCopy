using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolarWinds.SharedCommunication.Contracts.Utils
{
    /// <summary>
    /// An interface for AsyncSemaphore.
    /// </summary>
    public interface IAsyncSemaphore : IDisposable
    {
        /// <summary>
        /// A wrapper of LockAsync task based on calcellation token.
        /// </summary>
        /// <param name="token"></param>
        Task WaitAsync(CancellationToken token = default);

        /// <summary>
        /// A task for locking the resource based on calcellation token.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        Task<IDisposable> LockAsync(CancellationToken token = default);

        /// <summary>
        /// A method for releasing the semaphore.
        /// </summary>
        void Release();
    }
}