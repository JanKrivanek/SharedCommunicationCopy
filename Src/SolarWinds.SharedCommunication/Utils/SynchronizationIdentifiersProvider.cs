using System;
using System.Security.Cryptography;
using System.Text;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.Utils
{
    /// <summary>
    /// A class for synchronization identifiers provider.
    /// </summary>
    public class SynchronizationIdentifiersProvider: ISynchronizationIdentifiersProvider
    {
        /// <summary>
        /// Gets synchronization identifier based on API base address, API key and organization identifier.
        /// </summary>
        /// <param name="apiBaseAddress">API base address.</param>
        /// <param name="apiKey">API key.</param>
        /// <param name="orgId">Organization identifier.</param>
        public string GetSynchronizationIdentifier(string apiBaseAddress, string apiKey, string orgId)
        {
            if (string.IsNullOrEmpty(apiBaseAddress))
                throw new ArgumentException("Parameter must be nonempty", nameof(apiBaseAddress));

            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Parameter must be nonempty", nameof(apiKey));

            string uniqueIdentity = apiBaseAddress + "_" + apiKey + (orgId == null ? null : ("_" + orgId));

            //it's better to randomize salt; on the other hand we must get consistent result across processes
            // so some common schema must be used. No salting might be acceptable as well - the identity
            // should be long and random enough to prevent against hashed dictionary attack.
            string salt = "k(Dcvw,F5LK;*[K~";

            //now hash to prevent info leaking
            var hashAlgo = new SHA256Managed();
            var hash = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(uniqueIdentity + salt));

            //this can now be used as identity of shared handles (memory mapped files etc.)
            string id = Convert.ToBase64String(hash);

            return id;
        }

        /// <summary>
        /// An overloaded method for getting synchronization identifier based on API base address and API key.
        /// </summary>
        /// <param name="apiBaseAddress">API base address.</param>
        /// <param name="apiKey">API key.</param>
        public string GetSynchronizationIdentifier(string apiBaseAddress, string apiKey)
        {
            return GetSynchronizationIdentifier(apiBaseAddress, apiKey, null);
        }
    }
}
