namespace SolarWinds.SharedCommunication.Contracts.Utils
{
    /// <summary>
    /// interface for async semaphore factory
    /// </summary>
    public interface IAsyncSemaphoreFactory
    {
        /// <summary>
        /// method for creating the async semaphore
        /// </summary>
        /// <param name="name"> semaphore name </param>
        /// <returns></returns>
        IAsyncSemaphore Create(string name);

        /// <summary>
        /// method for creating the async semaphore based in name and kernel object privileges checker
        /// </summary>
        /// <param name="name"> semaphore name </param>
        /// <param name="kernelObjectsPrivilegesChecker"> privileges checker </param>
        /// <returns></returns>
        IAsyncSemaphore Create(string name, IKernelObjectsPrivilegesChecker kernelObjectsPrivilegesChecker);
    }
}