namespace SolarWinds.SharedCommunication.Contracts.Utils
{
    /// <summary>
    /// interface for kernel objects privileges checker
    /// </summary>
    public interface IKernelObjectsPrivilegesChecker
    {
        bool CanWriteToGlobalNamespace { get; }
        string KernelObjectsPrefix { get; }
    }
}
