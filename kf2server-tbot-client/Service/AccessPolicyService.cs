using kf2server_tbot_client.Security;
using kf2server_tbot_client.ServerAdmin.AccessPolicy;
using kf2server_tbot_client.Utils;
using System;
using System.Linq;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

    /// <summary>
    /// WCF Service implementation for Access Policy category
    /// </summary>
    public class AccessPolicyService : ServiceTools, IAccessPolicyService {

        /// <summary>
        /// Sets an active game password for players joining.
        /// <para>If no parameter passed, then take from config (DefaultGamePassword)</para>
        /// <para>If config (DefaultGamePassword) is empty or null, then no password set (Open)</para>
        /// </summary>
        /// <param name="pwd">Game Password</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("AccessPolicy.GamePasswordOn")]
        public ResponseValue GamePasswordOn(string pwd = null) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GamePasswordOn").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    Passwords.Instance.GamePwd((string.IsNullOrEmpty(pwd)) ? Properties.Settings.Default.DefaultGamePassword : pwd);
                }));

                LogEngine.Log(Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}')", GetType().GetMethod("GamePasswordOn").
                    GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), pwd));

                return new ResponseValue(true, string.Format("Setting game password to ",
                    (string.IsNullOrEmpty(pwd)) ? "'" + pwd + "'" : "DefaultGamePassword from config"), null);


            } else {
                LogEngine.Log(Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }
        }


        /// <summary>
        /// Removes an active game password.
        /// </summary>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("AccessPolicy.GamePasswordOff")]
        public ResponseValue GamePasswordOff() {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GamePasswordOff").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    Passwords.Instance.GamePwd();
                }));

                LogEngine.Log(Status.SERVICE_INFO,
                    string.Format("{0} for {1}", GetType().GetMethod("GamePasswordOff").
                    GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP()));

                return new ResponseValue(true, string.Format("Removing game password"), null);


            } else {
                LogEngine.Log(Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }
        }
    }
}
