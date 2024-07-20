using Serilog;
using StrictParent.Common;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace StrictParent.Service.Services
{
    public class WCFService
    {
        ServiceHost host = null;
        private readonly ILogger _logger;
        public WCFService(ILogger logger)
        {
            _logger = logger;
        }

        public void OpenWCF()
        {
            if (host != null && host.State != CommunicationState.Closed)
            {
                _logger.Warning("Tried to open WCF but WCF was already open with");
                return;
            }

            host = new ServiceHost(typeof(WCFContract), new Uri("http://localhost:8022/UPService"));

            ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();

            if (smb == null)
                smb = new ServiceMetadataBehavior();

            host.Description.Behaviors.Add(smb);

            host.AddServiceEndpoint(typeof(IWCF), new BasicHttpBinding(), "");

            host.Open();
        }

        public void CloseWCF()
        {
            host?.Close();

            //if (host is null)
            //    Log.Information("Host was null");

            //if (host != null)
            //    Log.Information("WCF Host status = " + host.State);
        }
    }
}
