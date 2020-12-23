namespace SolarWinds.SharedCommunication.Contracts.Utils
{
    /// <summary>
    /// interface for synchronization identifiers provider
    /// </summary>
    public interface ISynchronizationIdentifiersProvider
    {
        /// <summary>
        /// gets synchronization identifier
        /// </summary>
        /// <param name="apiBaseAddress"> API base address</param>
        /// <param name="apiKey"> API key </param>
        /// <param name="orgId"> organization identifier </param>
        /// <returns></returns>
        string GetSynchronizationIdentifier(string apiBaseAddress, string apiKey, string orgId);

        /// <summary>
        /// overloaded method for getting synchronization identifies
        /// </summary>
        /// <param name="apiBaseAddress"> API base address </param>
        /// <param name="apiKey"> API key </param>
        /// <returns></returns>
        string GetSynchronizationIdentifier(string apiBaseAddress, string apiKey);
    }
}
