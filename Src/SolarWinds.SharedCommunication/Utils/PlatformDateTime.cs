using System;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.Utils
{
    /// <summary>
    /// A class for getting the platform datetime.
    /// </summary>
    public class PlatformDateTime : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}