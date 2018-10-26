using System;

namespace kf2server_tbot_client.ServerAdmin.Settings {

    /// <summary>
    /// Not implemented
    /// </summary>
    class GameTypes : WebminPage {

        #region Singleton Structure
        private static GameTypes instance = null;
        private static readonly object padlock = new object();

        public static GameTypes Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new GameTypes();
                    }
                    return instance;
                }
            }
        }

        GameTypes() { }
        #endregion

        public override Tuple<bool, string> Init() {

            OpenPage(Properties.Settings.Default.GameTypesURL);

            PageManager.Pages[PageType.GameTypes] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
            WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

            return new Tuple<bool, string>(true, null);
        }
    }

}
