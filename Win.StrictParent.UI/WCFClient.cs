using StrictParent.Common;
using StrictParent.Common.DTOs;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace StrictParent.UI
{
    public class WCFClient : ClientBase<IWCF>, IWCF
    {
        public WCFClient(BasicHttpBinding binding, EndpointAddress address)
         : base(binding, address)
        {

        }

        public async Task<StatusResponseDto> CheckStatus(SettingsDto settings) => await Channel.CheckStatus(settings);
        public SettingsDto GetSettings() => Channel.GetSettings();
        public DateTime[] ParseCorrectDateTime(DateTime now, string fromTimeStr, string untilTimeStr) =>
            Channel.ParseCorrectDateTime(now, fromTimeStr, untilTimeStr);
        public void SaveSettings(SettingsDto settings) => Channel.SaveSettings(settings);
        public void SettingsChanged() => Channel.SettingsChanged();
    }
}