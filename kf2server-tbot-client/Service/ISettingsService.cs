using kf2server_tbot_client.Utils;
using System;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISettingsService" in both code and config file together.
    [ServiceContract]
    public interface ISettingsService {

        [OperationContract]
        ResponseValue General_Game_GameDifficulty(string difficulty);

        [OperationContract]
        ResponseValue General_Game_GameLength(string length);

    }
}
