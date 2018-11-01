using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kf2server_tbot_client.ServerAdmin.CurrentGame {

    class Players : WebminPage{

        #region Singleton Structure
        private static Players instance = null;
        private static readonly object padlock = new object();

        public static Players Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new Players();
                    }
                    return instance;
                }
            }
        }

        Players() { }
        #endregion

        public override Tuple<bool, string> Init() {

            OpenPage(Properties.Settings.Default.PlayersURL, "//*[@id=\"chatwindowframe\"]");

            PageManager.Pages[PageType.Players] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
            WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

            return new Tuple<bool, string>(true, null);
        }

    }

}
