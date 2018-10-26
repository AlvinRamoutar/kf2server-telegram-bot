using System;
using System.ServiceModel;

namespace TWCFClient.Service {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISettingsService" in both code and config file together.
    [ServiceContract]
    public interface ISettingsService {

        [OperationContract]
        ResponseValue General_Game_GameDifficulty(string difficulty);

        [OperationContract]
        ResponseValue General_Game_GameLength(string length);

    }
}
