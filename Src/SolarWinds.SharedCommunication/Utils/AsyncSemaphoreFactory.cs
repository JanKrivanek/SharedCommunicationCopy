using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using SolarWinds.SharedCommunication.Contracts.Utils;
using Microsoft.Extensions.Logging;

namespace SolarWinds.SharedCommunication.Utils
{
    /// <summary>
    /// A class for async semaphore factory.
    /// </summary>
    public class AsyncSemaphoreFactory : IAsyncSemaphoreFactory
    {
        private readonly ILogger _logger;

        public AsyncSemaphoreFactory(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// A method for creating the async semaphore based on semaphore name.
        /// </summary>
        /// <param name="name">Semaphore name.</param>
        public IAsyncSemaphore Create(string name)
        {
            return this.Create(name, KernelObjectsPrivilegesChecker.GetInstance(_logger));
        }

        /// <summary>
        /// A method for creating the async semaphore based on name and kernel object privileges checker.
        /// </summary>
        /// <param name="name">Semaphore name.</param>
        /// <param name="kernelObjectsPrivilegesChecker">Privileges checker.</param>
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