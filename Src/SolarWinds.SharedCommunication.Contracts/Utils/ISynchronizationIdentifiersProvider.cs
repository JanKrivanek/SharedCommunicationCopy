namespace SolarWinds.SharedCommunication.Contracts.Utils
{
    public interface ISynchronizationIdentifiersProvider
    {
        string GetSynchronizationIdentifier(string apiBaseAddress, string apiKey, string orgId);
        string GetSynchronizationIdentifier(string apiBaseAddress, string apiKey);
    }
}
