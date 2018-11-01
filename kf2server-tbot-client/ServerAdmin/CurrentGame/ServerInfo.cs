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

            OpenPage(Properties.Settings.Default.ServerInfoURL, "//*[@id=\"chatwindowframe\"]");

            PageManager.Pages[PageType.ServerInfo] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
            WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

            return new Tuple<bool, string>(true, null);
        }

    }

}
