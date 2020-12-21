using System.ServiceModel;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true,
        AutomaticSessionShutdown = true,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PollerDataCacheService : PollerDataCacheImpl
    {
        private ServiceHost service;

        public PollerDataCacheService(IDateTime dateTime) : base(dateTime)
        { }

        public void Start()
        {
            service = new ServiceHost(this);
            service.Open();
        }

        public void Shutdown()
        {
            service.Close();
        }
    }
}