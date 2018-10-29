using kf2server_tbot_client.Utils;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

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
        ResponseValue Players();

        [OperationContract]
        ResponseValue Players_KickPlayer();

    }
}
