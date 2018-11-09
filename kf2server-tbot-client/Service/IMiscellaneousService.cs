using kf2server_tbot_client.Utils;
using System;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

    /// <summary>
    /// Interface containing definitions of service methods for operations not in a specific ServerAdmin category
    /// </summary>
    [ServiceContract]
    public interface IMiscellaneousService {


        /// <summary>
        /// Submits a message through Chat Console directly to game under user 'admin'
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>ResponseValue object</returns>
        [OperationContract]
        ResponseValue AdminSay(string message);

        [OperationContract]
        ResponseValue Pause();

        [OperationContract]
        ResponseValue AddUser(string telegramUUID, string[] roles = null);

        [OperationContract]
        ResponseValue RemoveUser(string telegramUUID, string[] roles = null);

        [OperationContract]
        ResponseValue Test();

    }
}
