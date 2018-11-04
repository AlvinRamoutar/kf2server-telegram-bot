using kf2server_tbot_client.Utils;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

    [ServiceContract]
    public interface ISettingsService {

        [OperationContract]
        ResponseValue General_Game_GameDifficulty(string difficulty);

        [OperationContract]
        ResponseValue General_Game_GameLength(string length);

        [OperationContract]
        ResponseValue General_Game_GameDifficultyAndLength(string difficulty, string length);
    }
}
