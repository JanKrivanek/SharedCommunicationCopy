namespace SolarWinds.SharedCommunication.Contracts.Utils
{
    /// <summary>
    /// An interface for synchronization identifiers provider.
    /// </summary>
    public interface ISynchronizationIdentifiersProvider
    {
        /// <summary>
        /// Gets synchronization identifier based on API base address, API key and organization identifier.
        /// </summary>
        /// <param name="apiBaseAddress">API base address.</param>
        /// <param name="apiKey">API key.</param>
        /// <param name="orgId">Organization identifier.</param>
        string GetSynchronizationIdentifier(string apiBaseAddress, string apiKey, string orgId);

        /// <summary>
        /// An overloaded method for getting synchronization identifier based on API base address and API key.
        /// </summary>
        /// <param name="apiBaseAddress">API base address.</param>
        /// <param name="apiKey">API key.</param>
        string GetSynchronizationIdentifier(string apiBaseAddress, string apiKey);
    }
}
