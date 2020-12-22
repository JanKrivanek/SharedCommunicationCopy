using System;
using System.IO.MemoryMappedFiles;
using SolarWinds.SharedCommunication.Contracts.Utils;
using Microsoft.Extensions.Logging;

namespace SolarWinds.SharedCommunication.Utils
{
    public class KernelObjectsPrivilegesChecker : IKernelObjectsPrivilegesChecker
    {
        public bool CanWriteToGlobalNamespace => instance.CanWriteToGlobalNamespace;
        public string KernelObjectsPrefix => instance.KernelObjectsPrefix;
        private static IKernelObjectsPrivilegesChecker instance;

        public static IKernelObjectsPrivilegesChecker GetInstance(ILogger logger)
        {
            //no synchro needed here; we're fine with race
            if (instance == null)
            {
                instance = new KernelObjectsPrivilegesCheckerImpl(logger);
            }

            return instance;
        }

        public KernelObjectsPrivilegesChecker(ILogger logger)
        {
            GetInstance(logger);
        }

        private class KernelObjectsPrivilegesCheckerImpl : IKernelObjectsPrivilegesChecker
        {
            private const string globalNamespacePrefix = "Global\\";

            public bool CanWriteToGlobalNamespace { get; }
            public string KernelObjectsPrefix { get; }

            public KernelObjectsPrivilegesCheckerImpl(ILogger logger)
            {
                CanWriteToGlobalNamespace = CanCreateMmfInGlobalNamespace(logger);
                KernelObjectsPrefix =
                    CanWriteToGlobalNamespace ? globalNamespacePrefix : string.Empty;
            }

            private bool CanCreateMmfInGlobalNamespace(ILogger logger)
            {
                try
                {
                    var f = MemoryMappedFile.CreateNew(globalNamespacePrefix + Guid.NewGuid(), 1);
                    f.Dispose();
                }
                catch (UnauthorizedAccessException e)
                {
                    logger.LogError(
                        $"Cannot write into Global kernel namespace. Falling back to creating and opening objects without namespace prefix " +
                        $"(so proper communication/synchronization is limit to just single windows session). To prevent this, make sure the process is running with appropriate privileges",
                        e);
                    return false;
                }

                return true;
            }
        }
    }
}
