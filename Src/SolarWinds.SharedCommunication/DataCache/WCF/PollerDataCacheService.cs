using System.ServiceModel;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    /// <summary>
    /// a class for poller data cache servise
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true,
        AutomaticSessionShutdown = true,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PollerDataCacheService : PollerDataCache
    {
        private ServiceHost service;

        public PollerDataCacheService(IDateTime dateTime) : base(dateTime)
        { }

        /// <summary>
        /// starts the service
        /// </summary>
        public void Start()
        {
            service = new ServiceHost(this);
            service.Open();
        }

        //shuts the service down
        public void Shutdown()
        {
            service.Close();
        }
    }
}