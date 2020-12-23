using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolarWinds.SharedCommunication.Contracts.Utils
{
    /// <summary>
    /// interface for AsyncSemaphore
    /// </summary>
    public interface IAsyncSemaphore : IDisposable
    {
        /// <summary>
        /// wrapper of LockAsync task
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task WaitAsync(CancellationToken token = default);

        /// <summary>
        /// task for locking the resource
        /// </summary>
        /// <param name="token"> cancellation token </param>
        /// <returns></returns>
        Task<IDisposable> LockAsync(CancellationToken token = default);

        /// <summary>
        /// method for releasing the semaphore
        /// </summary>
        void Release();
    }
}