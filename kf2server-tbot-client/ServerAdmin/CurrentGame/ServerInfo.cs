using kf2server_tbot_client.Utils;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kf2server_tbot_client.ServerAdmin.CurrentGame {

    class ServerInfo : WebminPage {

        #region Singleton Structure
        private static ServerInfo instance = null;
        private static readonly object padlock = new object();

        public static ServerInfo Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new ServerInfo();
                    }
                    return instance;
                }
            }
        }

        ServerInfo() { }
        #endregion

        public override Tuple<bool, string> Init() {

            try {
                OpenPage(Properties.Settings.Default.ServerInfoURL, "//*[@id=\"chatwindowframe\"]");

                PageManager.Pages[PageType.ServerInfo] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

                LogEngine.Log(Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded ServerInfo page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch(NoSuchElementException nsee) {
                LogEngine.Log(Status.PAGELOAD_FAILURE, "Failed to load ServerInfo page");
                return new Tuple<bool, string>(false, nsee.Message);
            }

            
        }

    }

}
