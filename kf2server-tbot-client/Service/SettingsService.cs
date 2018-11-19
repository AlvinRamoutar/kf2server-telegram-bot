using kf2server_tbot_client.Security;
using kf2server_tbot_client.ServerAdmin.Settings;
using kf2server_tbot_client.Utils;
using System;
using System.Linq;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

    /// <summary>
    /// WCF Service implementation for Settings category
    /// </summary>
    public class SettingsService : ServiceTools, ISettingsService {

        /// <summary>
        /// Change Game Difficulty service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Settings.GameDifficulty")]
        public ResponseValue GameDifficulty(string difficulty) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameDifficulty").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameDifficulty(difficulty);
                }));


                LogEngine.Log(Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}')", GetType().GetMethod("GameDifficulty")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), difficulty));

                return new ResponseValue(true, string.Format("Changing game difficulty to: {0}", difficulty), null);


            } else {
                LogEngine.Log(Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        /// <summary>
        /// Change Game Length service method
        /// </summary>
        /// <param name="length">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Settings.GameLength")]
        public ResponseValue GameLength(string length) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameLength").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameLength(length);
                }));


                LogEngine.Log(Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}')", GetType().GetMethod("GameLength")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), length));


                return new ResponseValue(true, string.Format("Changing game length to: {0}", length), null);


            } else {
                LogEngine.Log(Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        /// <summary>
        /// Change Game Difficulty and Length service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <param name="length">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Settings.GameDifficultyAndLength")]
        public ResponseValue GameDifficultyAndLength(string difficulty, string length) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameDifficultyAndLength").GetCustomAttributes(true)
                    .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameDifficultyAndLength(difficulty, length);
                }));


                LogEngine.Log(Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}, {3}')", GetType().GetMethod("GameDifficultyAndLength")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), difficulty, length));

                return new ResponseValue(true, string.Format("Changing game difficulty to: {0} and length to: {1}", difficulty, length), null);


            } else {
                LogEngine.Log(Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }
        }
    }
}
