using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using kf2server_tbot_client.Security;
using kf2server_tbot_client.ServerAdmin;
using kf2server_tbot_client.ServerAdmin.CurrentGame;
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

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("AdminSay").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ChatConsole.SendMessage(message);
                }));


                LogEngine.Log(Utils.Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}')", GetType().GetMethod("AdminSay").GetCustomAttributes(true)
                        .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), message));

                return new ResponseValue(true, message, null);


            } else {
                LogEngine.Log(Utils.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }
        }



        [ServiceMethodRoleID("Miscellaneous.Pause")]
        public ResponseValue Pause() {
            throw new NotImplementedException();
        }



        [ServiceMethodRoleID("Miscellaneous.AddUser")]
        public ResponseValue AddUser(string telegramUUID, string[] roles = null) {

            /*
            Console.WriteLine(OperationContext.Current.RequestContext.RequestMessage.ToString());
            foreach(var header in OperationContext.Current.RequestContext.RequestMessage.Headers) {
                Console.WriteLine(header.ToString());
            }
            */

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("AddUser").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);

            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    try {
                        bool doesUserExist = Users.AddUser(AuthManager.Users, telegramUUID, roles);
                    } catch (Exception) { }
                }));


                LogEngine.Log(Utils.Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}', '{3}')", GetType().GetMethod("AddUser")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), telegramUUID, JsonConvert.SerializeObject(roles)));


                return new ResponseValue(true, "New User Added", new Dictionary<string, string>() {
                    { "roles", (roles != null) ? JsonConvert.SerializeObject(roles) : string.Empty }
                });


            } else {
                LogEngine.Log(Utils.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }



        [ServiceMethodRoleID("MiscellaneousService.RemoveUser")]
        public ResponseValue RemoveUser(string telegramUUID, string[] roles = null) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("RemoveUser").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);

            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    try {
                        Users.RemoveUser(AuthManager.Users, telegramUUID, roles);
                    } catch (Exception) { }
                }));


                LogEngine.Log(Utils.Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}', '{3}')", GetType().GetMethod("RemoveUser")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), telegramUUID, JsonConvert.SerializeObject(roles)));


                return new ResponseValue(true, "User or roles were removed", new Dictionary<string, string>() {
                    { "roles", (roles != null) ? JsonConvert.SerializeObject(roles) : string.Empty }
                });


            } else {
                LogEngine.Log(Utils.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }



        [ServiceMethodRoleID("Miscellaneous.Status")]
        public ResponseValue Status() {

            System.Threading.ManualResetEvent syncEvent = new System.Threading.ManualResetEvent(false);

            Tuple<bool, Dictionary<string, string>> Result = new Tuple<bool, Dictionary<string, string>>(false, null);

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                Result = ServerInfo.Instance.Status();
                syncEvent.Set();
            }));

            syncEvent.WaitOne();

            if(!Result.Item1) {

                LogEngine.Log(Utils.Status.SERVICE_FAILURE, "Failed to retrieve status information from ServerInfo");

                return new ResponseValue(false, null, null);

            } else {

                LogEngine.Log(Utils.Status.SERVICE_INFO,
                    string.Format("{0} from {1}", GetType().GetMethod("Status")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, GetIP()));

                return new ResponseValue(true, string.Empty, Result.Item2);
            }
        }



        [ServiceMethodRoleID("Miscellaneous.Test")]
        public ResponseValue Test() {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("Test").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);

            if (AuthResult.Item1) {

                LogEngine.Log(Utils.Status.SERVICE_INFO,
                    string.Format("{0} from {1}", GetType().GetMethod("Test")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, GetIP()));

                return new ResponseValue(true, "This is a sample action, occuring on successful authentication.", 
                    new Dictionary<string, string>() { { "errors", null } });

            } else {
                LogEngine.Log(Utils.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }
            
        }

    }
}
