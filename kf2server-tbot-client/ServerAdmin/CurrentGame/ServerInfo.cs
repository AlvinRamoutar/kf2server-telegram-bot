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

        private Dictionary<string, string> SummaryElements = new Dictionary<string, string>();

        public override Tuple<bool, string> Init() {

            try {
                OpenPage(Properties.Settings.Default.ServerInfoURL, "//*[@id=\"chatwindowframe\"]");

                PageManager.Pages[PageType.ServerInfo] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

                /// Dictionary of selectors (xpath) and values for summary elements (used primarily for Status cmd)
                SummaryElements = new Dictionary<string, string>() {
                    { "ServerName", "/html/body/div[4]/table/tbody/tr/td[1]/div/dl/dd[1]" },
                    { "GameType", "/html/body/div[4]/table/tbody/tr/td[1]/div/dl/dd[3]" },
                    { "Map", "/html/body/div[4]/table/tbody/tr/td[1]/div/dl/dd[4]" },
                    { "Difficulty", "/html/body/div[4]/table/tbody/tr/td[2]/div/dl/dd[2]" },
                    { "Wave", "/html/body/div[4]/table/tbody/tr/td[2]/div/dl/dd[1]" },
                    { "Players", "/html/body/div[4]/table/tbody/tr/td[2]/div/dl/dd[3]" },
                    { "Spectators", "/html/body/div[4]/table/tbody/tr/td[2]/div/dl/dd[5]" }
                };

                LogEngine.Log(Utils.Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded ServerInfo page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch(NoSuchElementException nsee) {
                LogEngine.Log(Utils.Status.PAGELOAD_FAILURE, "Failed to load ServerInfo page");
                return new Tuple<bool, string>(false, nsee.Message);
            }
        }


        public Tuple<bool, Dictionary<string, string>> Status() {

            try {

                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                Driver.Navigate().Refresh();

                Dictionary<string, string> tmpSummaryDict = new Dictionary<string, string>();
                
                /// Retrieve each value as identified in SummaryElements dict, via XPaths
                foreach(KeyValuePair<string, string> elem in SummaryElements) {
                    tmpSummaryDict.Add(elem.Key, Driver.FindElement(By.XPath(elem.Value)).Text);
                }

                return new Tuple<bool, Dictionary<string, string>>(true, tmpSummaryDict);

            } catch (NoSuchElementException) {
                LogEngine.Log(Utils.Status.PAGELOAD_FAILURE, "Failed to retrieve status info from ServerInfo page");
                return new Tuple<bool, Dictionary<string, string>>(false, null);
            }

        }


    }

}
