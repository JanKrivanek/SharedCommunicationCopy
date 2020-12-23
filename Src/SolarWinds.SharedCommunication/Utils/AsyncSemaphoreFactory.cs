using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using SolarWinds.SharedCommunication.Contracts.Utils;
using Microsoft.Extensions.Logging;

namespace SolarWinds.SharedCommunication.Utils
{
    /// <summary>
    /// class for async semaphore factory
    /// </summary>
    public class AsyncSemaphoreFactory : IAsyncSemaphoreFactory
    {
        private readonly ILogger _logger;

        public AsyncSemaphoreFactory(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// method for creating the async semaphore
        /// </summary>
        /// <param name="name"> semaphore name </param>
        /// <returns></returns>
        public IAsyncSemaphore Create(string name)
        {
            return this.Create(name, KernelObjectsPrivilegesChecker.GetInstance(_logger));
        }

        /// <summary>
        /// method for creating the async semaphore based in name and kernel object privileges checker
        /// </summary>
        /// <param name="name"> semaphore name </param>
        /// <param name="kernelObjectsPrivilegesChecker"> privileges checker </param>
        /// <returns></returns>
        public IAsyncSemaphore Create(string name, IKernelObjectsPrivilegesChecker kernelObjectsPrivilegesChecker)
        {
            var allowEveryoneRule = new SemaphoreAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                SemaphoreRights.FullControl, AccessControlType.Allow);
            SemaphoreSecurity securitySettings = new SemaphoreSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            name = kernelObjectsPrivilegesChecker.KernelObjectsPrefix + name;

            bool createdNew;
            Semaphore sp = new Semaphore(1, 1, name, out createdNew, securitySettings);

            string action = createdNew ? "Created new" : "Opened existing";
            _logger.LogDebug("{action} Semaphore with name {name}.", action, name);

            return new AsyncSemaphore(sp);
        }
    }
}