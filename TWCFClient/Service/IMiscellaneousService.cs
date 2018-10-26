using System;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace TWCFClient.Service {

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
