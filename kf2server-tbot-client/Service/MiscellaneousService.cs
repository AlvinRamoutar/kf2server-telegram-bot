using System;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Channels;
using kf2server_tbot_client.ServerAdmin;
using kf2server_tbot_client.Utils;

namespace kf2server_tbot_client.Service {

    public class MiscellaneousService : IMiscellaneousService {


        /// <summary>
        /// Submits a message through Chat Console directly to game under user 'admin'
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>Message sent</returns>
        [ServiceMethodRoleID("Miscellaneous.AdminSay")]
        public ResponseValue AdminSay(string message) {

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                ChatConsole.SendMessage(message);
            }));

            LogEngine.Log(Status.SERVICE_INFO, 
                string.Format("{0} ('{1}')", GetType().GetMethod("AdminSay").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, message));

            return new ResponseValue(true, string.Format("ADMIN: {0}", message), null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ServiceMethodRoleID("Miscellaneous.Pause")]
        public ResponseValue Pause() {

            /*
             * @todo Implement Pause service method
             * @body Need to first implement action in new ServiceAdmin page (just for console cmds)
             */
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
