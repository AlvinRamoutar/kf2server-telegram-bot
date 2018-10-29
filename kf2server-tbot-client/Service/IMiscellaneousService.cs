using kf2server_tbot_client.Utils;
using System;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace kf2server_tbot_client.Service {

    [ServiceContract]
    public interface IMiscellaneousService {

        [OperationContract]
        ResponseValue AdminSay(string message);

        [OperationContract]
        ResponseValue Pause();

        [OperationContract]
        ResponseValue Test();

    }
}
