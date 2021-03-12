using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Unplug.Common;

namespace Unplug.UI
{
    public class WCFClient : ClientBase<IWCF>, IWCF
    {
        public WCFClient(BasicHttpBinding binding, EndpointAddress address)
         : base(binding, address)
        {

        }

        public SettingsDto GetSettings()
        {
            return Channel.GetSettings();
        }

        public void SaveSettings(SettingsDto settings)
        {
            Channel.SaveSettings(settings);
        }

        public void SettingsChanged()
        {
            Channel.SettingsChanged();
        }
    }
}
