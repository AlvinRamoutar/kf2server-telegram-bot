using kf2server_tbot_client.Utils;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

    /// <summary>
    /// Interface containing definitions of service methods for Current Game category
    /// </summary>
    [ServiceContract]
    public interface ICurrentGameService {

        [OperationContract]
        ResponseValue Status();


        [OperationContract]
        ResponseValue ChangeGameType(string gametype);

        [OperationContract]
        ResponseValue ChangeMap(string map);

        [OperationContract]
        ResponseValue ChangeGametypeAndMap(string gametype, string map);


        [OperationContract]
        ResponseValue Online();

        [OperationContract]
        ResponseValue Kick(string playername);

    }
}
