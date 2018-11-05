using kf2server_tbot_client.Utils;
using System;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

    /// <summary>
    /// Interface containing definitions of service methods for Access Policy category
    /// </summary>
    [ServiceContract]
    public interface IAccessPolicyService {

        /// <summary>
        /// Sets an active game password for players joining.
        /// <para>If no parameter passed, then take from config (DefaultGamePassword)</para>
        /// <para>If config (DefaultGamePassword) is empty or null, then no password set (Open)</para>
        /// </summary>
        /// <param name="pwd">Game Password</param>
        /// <returns>ResponseValue object</returns>
        [OperationContract]
        ResponseValue GamePasswordOn(string pwd = null);


        /// <summary>
        /// Removes an active game password.
        /// </summary>
        /// <returns>ResponseValue object</returns>
        [OperationContract]
        ResponseValue GamePasswordOff();
    }
}
