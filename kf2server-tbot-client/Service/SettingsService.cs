using kf2server_tbot_client.ServerAdmin.Settings;
using kf2server_tbot_client.Utils;
using System.Linq;

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
        [ServiceMethodRoleID("Settings.General_Game_GameDifficulty")]
        public ResponseValue General_Game_GameDifficulty(string difficulty) {

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                General.Instance.ChangeGameDifficulty(difficulty);
            }));

            LogEngine.Log(Status.SERVICE_INFO,
                string.Format("{0} from {1} ('{2}')", GetType().GetMethod("General_Game_GameDifficulty")
                .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, 
                GetIP(), difficulty));

            return new ResponseValue(true, string.Format("Changing game difficulty to: {0}", difficulty), null);
        }


        /// <summary>
        /// Change Game Length service method
        /// </summary>
        /// <param name="length">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Settings.General_Game_GameLength")]
        public ResponseValue General_Game_GameLength(string length) {

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                General.Instance.ChangeGameLength(length);
            }));

            LogEngine.Log(Status.SERVICE_INFO,
                string.Format("{0} from {1} ('{2}')", GetType().GetMethod("General_Game_GameLength")
                .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, 
                GetIP(), length));


            return new ResponseValue(true, string.Format("Changing game difficulty to: {0}", length), null);

        }


        /// <summary>
        /// Change Game Difficulty and Length service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <param name="length">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Settings.General_Game_GameDifficultyAndLength")]
        public ResponseValue General_Game_GameDifficultyAndLength(string difficulty, string length) {

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                General.Instance.ChangeGameDifficultyAndLength(difficulty, length);
            }));

            LogEngine.Log(Status.SERVICE_INFO,
                string.Format("{0} from {1} ('{2}, {3}')", GetType().GetMethod("General_Game_GameDifficultyAndLength")
                .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, 
                GetIP(), difficulty, length));

            return new ResponseValue(true, string.Format("Changing game difficulty to: {0} and length to: {1}", difficulty, length), null);

        }
    }
}
