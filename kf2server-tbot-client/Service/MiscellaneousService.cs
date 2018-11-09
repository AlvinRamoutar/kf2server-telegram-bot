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

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {

                Account newUser = new Account();

                bool DoesUserExist = false;

                /// Check: Does user already exist?
                try {
                    AuthManager.Users[telegramUUID].ToString();
                    DoesUserExist = true;
                } catch (NullReferenceException) { }


                if(DoesUserExist) {

                    Role roleContainer = new Role();
                    newUser.TelegramUUID = telegramUUID;
                    List<string> tmpNewUserRoles = new List<string>();

                    /// Create new Role object with all supplied roles
                    foreach(string r in roles) {
                        if (Users.Roles.RoleID.Contains(r)) {
                            tmpNewUserRoles.Add(r);
                        }
                    }

                    /// Assign roles collection to newly created roles object
                    roleContainer.RoleID = tmpNewUserRoles.ToArray<string>();
                    newUser.Roles = roleContainer;

                } else { /// This user doesn't exist

                    newUser = AuthManager.Users[telegramUUID];
                    List<string> tmpNewUserRoles = new List<string>(newUser.Roles.RoleID);

                    /// Add role to users' role collection if they don't aready have it, and it exists (is a valid role)
                    foreach(string r in roles) {
                        if(Users.Roles.RoleID.Contains(r) && !tmpNewUserRoles.Contains(r)) {
                            tmpNewUserRoles.Add(r);
                        }
                    }

                    /// Replaces users role collection with temp role collection (representing deltas)
                    newUser.Roles.RoleID = tmpNewUserRoles.ToArray<string>();

                }


                AuthManager.Users.Accounts.Add(newUser);

            }));


            LogEngine.Log(Status.SERVICE_INFO, 
                string.Format("{0} from {1} ('{2}', '{3}')", GetType().GetMethod("AddUser")
                .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                GetIP(), telegramUUID, JsonConvert.SerializeObject(roles)));

            return new ResponseValue(true, "New User Added", new Dictionary<string, string>() {
                { "roles", (roles != null) ? JsonConvert.SerializeObject(roles) : string.Empty }
            });
        }



        [ServiceMethodRoleID("MiscellaneousService.Removeuser")]
        public ResponseValue RemoveUser(string telegramUUID, string[] roles = null) {
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
