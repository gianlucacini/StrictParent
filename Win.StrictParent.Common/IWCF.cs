using StrictParent.Common.DTOs;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace StrictParent.Common
{
    [ServiceContract]
    public interface IWCF
    {
        [OperationContract] void SettingsChanged();
        [OperationContract] SettingsDto GetSettings();
        [OperationContract] void SaveSettings(SettingsDto settings);
        [OperationContract] Task<StatusResponseDto> CheckStatus(SettingsDto settings);
        [OperationContract] DateTime[] ParseCorrectDateTime(DateTime now, String fromTimeStr, String untilTimeStr);
    }
}