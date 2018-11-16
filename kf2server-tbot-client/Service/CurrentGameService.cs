using kf2server_tbot_client.Security;
using kf2server_tbot_client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

    /// <summary>
    /// WCF Service implementation for Current Game category
    /// </summary>
    public class CurrentGameService : ServiceTools, ICurrentGameService {

        [ServiceMethodRoleID("CurrentGame.ChangeGameType")]
        public ResponseValue ChangeGameType(string gametype) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeGameType").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeGameTypeOnly(gametype);
                }));


                LogEngine.Log(Utils.Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}')", GetType().GetMethod("ChangeGameType").GetCustomAttributes(true)
                        .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), gametype));

                return new ResponseValue(true, string.Format("Changing game type to: {0}", gametype), null);

            } else {
                LogEngine.Log(Utils.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        [ServiceMethodRoleID("CurrentGame.ChangeGametypeAndMap")]
        public ResponseValue ChangeGametypeAndMap(string gametype, string map) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeGametypeAndMap").GetCustomAttributes(true)
                .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeMapAndGameType(gametype, map);
                }));


                LogEngine.Log(Utils.Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}', '{3}')", GetType().GetMethod("ChangeGametypeAndMap").GetCustomAttributes(true)
                        .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), gametype, map));

                return new ResponseValue(true, string.Format("Changing game type to: {0} and map to: {1}", gametype, map), null);

            } else {
                LogEngine.Log(Utils.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        [ServiceMethodRoleID("CurrentGame.ChangeMap")]
        public ResponseValue ChangeMap(string map) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeMap").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeMapOnly(map);
                }));


                LogEngine.Log(Utils.Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}')", GetType().GetMethod("ChangeMap").GetCustomAttributes(true)
                        .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), map));

                return new ResponseValue(true, string.Format("Changing map to: {0}", map), null);

            } else {
                LogEngine.Log(Utils.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        [ServiceMethodRoleID("CurrentGame.Players")]
        public ResponseValue Players() {
            throw new NotImplementedException();
        }


        [ServiceMethodRoleID("CurrentGame.Players_KickPlayer")]
        public ResponseValue Players_KickPlayer() {
            throw new NotImplementedException();
        }


        [ServiceMethodRoleID("CurrentGame.Status")]
        public ResponseValue Status() {
            throw new NotImplementedException();
        }
    }
}
