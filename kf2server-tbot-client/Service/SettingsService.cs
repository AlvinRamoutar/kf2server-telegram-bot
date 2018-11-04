using kf2server_tbot_client.ServerAdmin.Settings;
using kf2server_tbot_client.Utils;
using System;
using System.Linq;

namespace kf2server_tbot_client.Service {

    public class SettingsService : ISettingsService {


        [ServiceMethodRoleID("Settings.General_Game_GameDifficulty")]
        public ResponseValue General_Game_GameDifficulty(string difficulty) {

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                General.Instance.ChangeGameDifficulty(difficulty);
            }));

            LogEngine.Log(Status.SERVICE_INFO,
                string.Format("{0} ('{1}')", GetType().GetMethod("General_Game_GameDifficulty").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, difficulty));

            return new ResponseValue(true, string.Format("Changing game difficulty to: {0}", difficulty), null);
        }


        [ServiceMethodRoleID("Settings.General_Game_GameLength")]
        public ResponseValue General_Game_GameLength(string length) {

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                General.Instance.ChangeGameLength(length);
            }));

            LogEngine.Log(Status.SERVICE_INFO,
                string.Format("{0} ('{1}')", GetType().GetMethod("General_Game_GameLength").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, length));

            return new ResponseValue(true, string.Format("Changing game difficulty to: {0}", length), null);

        }


        [ServiceMethodRoleID("Settings.General_Game_GameDifficultyAndLength")]
        public ResponseValue General_Game_GameDifficultyAndLength(string difficulty, string length) {

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                General.Instance.ChangeGameDifficultyAndLength(difficulty, length);
            }));

            LogEngine.Log(Status.SERVICE_INFO,
                string.Format("{0} ('{1}, {2}')", GetType().GetMethod("General_Game_GameDifficultyAndLength").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, difficulty, length));

            return new ResponseValue(true, string.Format("Changing game difficulty to: {0} and length to: {1}", difficulty, length), null);

        }
    }
}
