namespace SolarWinds.SharedCommunication.Contracts.Utils
{
    /// <summary>
    /// An interface for async semaphore factory.
    /// </summary>
    public interface IAsyncSemaphoreFactory
    {
        /// <summary>
        /// A method for creating the async semaphore based on semaphore name.
        /// </summary>
        /// <param name="name">Semaphore name.</param>
        /// <returns>The created semaphore.</returns>
        IAsyncSemaphore Create(string name);

        /// <summary>
        /// A method for creating the async semaphore based in name and kernel object privileges checker based on semaphore name and privileges checker.
        /// </summary>
        /// <param name="name">Semaphore name.</param>
        /// <param name="kernelObjectsPrivilegesChecker">Privileges checker.</param>
        /// <returns>The created semaphore.</returns>
        IAsyncSemaphore Create(string name, IKernelObjectsPrivilegesChecker kernelObjectsPrivilegesChecker);
    }
}