using System;
using System.Linq;
using System.ServiceModel;
using kf2server_tbot_client.ServerAdmin;
using kf2server_tbot_client.Utils;

namespace kf2server_tbot_client.Service {


    /// <summary>
    /// WCF Service implementation for operations not in a specific ServerAdmin category
    /// </summary>
    public class MiscellaneousService : ServiceTools, IMiscellaneousService {


        /// <summary>
        /// Submits a message through Chat Console directly to game under user 'admin'
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Miscellaneous.AdminSay")]
        public ResponseValue AdminSay(string message) {

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                ChatConsole.SendMessage(message);
            }));

            LogEngine.Log(Status.SERVICE_INFO, 
                string.Format("{0} for {1} ('{2}')", GetType().GetMethod("AdminSay").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, 
                GetIP(), message));

            return new ResponseValue(true, message, null);
        }



        [ServiceMethodRoleID("Miscellaneous.Pause")]
        public ResponseValue Pause() {
            throw new NotImplementedException();
        }




        [ServiceMethodRoleID("Miscellaneous.Test")]
        public ResponseValue Test() {

            if (Security.AuthManager.Authorize(
                GetType().GetMethod("Test").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current)) {

                return new ResponseValue(true, Security.Crypto.Hash("This is my Telegram ID."), 
                    new System.Collections.Generic.Dictionary<string, string>() { { "errors", null } });

            } else {
                throw new FaultException("You don't have the privilege to perform this action.");
            }
            
        }

    }
}
