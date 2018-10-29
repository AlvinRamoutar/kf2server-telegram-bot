using kf2server_tbot_client.Utils;
using System;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

    [ServiceContract]
    public interface IAccessPolicyService {

        /// <summary>
        /// Sets an active game password for players joining.
        /// If no parameter passed, then take from config (DefaultGamePassword)
        /// If config (DefaultGamePassword) is empty or null, then no password set (Open)
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseValue GamePasswordOn(string pwd = null);


        [OperationContract]
        ResponseValue GamePasswordOff();
    }
}
