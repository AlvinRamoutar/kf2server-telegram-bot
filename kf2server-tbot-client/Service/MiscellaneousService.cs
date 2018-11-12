using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using kf2server_tbot_client.Security;
using kf2server_tbot_client.ServerAdmin;
using kf2server_tbot_client.Utils;
using Newtonsoft.Json;

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



        [ServiceMethodRoleID("Miscellaneous.AddUser")]
        public ResponseValue AddUser(string telegramUUID, string[] roles = null) {

            Console.WriteLine(OperationContext.Current.RequestContext.RequestMessage.ToString());
            foreach(var header in OperationContext.Current.RequestContext.RequestMessage.Headers) {
                Console.WriteLine(header.ToString());
            }

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {

                try {
                    bool doesUserExist = Users.AddUser(AuthManager.Users, telegramUUID, roles);
                } catch(Exception) { }

            }));


            LogEngine.Log(Status.SERVICE_INFO, 
                string.Format("{0} from {1} ('{2}', '{3}')", GetType().GetMethod("AddUser")
                .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                GetIP(), telegramUUID, JsonConvert.SerializeObject(roles)));

            return new ResponseValue(true, "New User Added", new Dictionary<string, string>() {
                { "roles", (roles != null) ? JsonConvert.SerializeObject(roles) : string.Empty }
            });
        }



        [ServiceMethodRoleID("MiscellaneousService.RemoveUser")]
        public ResponseValue RemoveUser(string telegramUUID, string[] roles = null) {
            throw new NotImplementedException();
        }


        [ServiceMethodRoleID("Miscellaneous.Test")]
        public ResponseValue Test() {

            if (AuthManager.Authorize(
                GetType().GetMethod("Test").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current)) {

                return new ResponseValue(true, "This is a sample action, occuring on successful authentication.", 
                    new Dictionary<string, string>() { { "errors", null } });

            } else {
                throw new FaultException("You don't have the privilege to perform this action.");
            }
            
        }

    }
}
