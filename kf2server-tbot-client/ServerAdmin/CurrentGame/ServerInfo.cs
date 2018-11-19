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

        private List<string> SummaryElementsToLookoutFor_CurrentGame = new List<string>();
        private List<string> SummaryElementsToLookoutFor_Rules = new List<string>();

        public override Tuple<bool, string> Init() {

            try {
                OpenPage(Properties.Settings.Default.ServerInfoURL, "//*[@id=\"chatwindowframe\"]");

                PageManager.Pages[PageType.ServerInfo] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

                /// Dictionary of selectors (xpath) and values for summary elements (used primarily for Status cmd)
                SummaryElements = new Dictionary<string, string>() {
                    { "ServerName", "/html/body/div[4]/table/tbody/tr/td[1]/div/dl/dd[1]" },
                    { "GameType",   "/html/body/div[4]/table/tbody/tr/td[1]/div/dl/dd[3]" },
                    { "Map",        "/html/body/div[4]/table/tbody/tr/td[1]/div/dl/dd[4]" },
                    { "Difficulty", "/html/body/div[4]/table/tbody/tr/td[2]/div/dl/dd[2]" },
                    { "Wave",       "/html/body/div[4]/table/tbody/tr/td[2]/div/dl/dd[1]" },
                    { "Players",    "/html/body/div[4]/table/tbody/tr/td[2]/div/dl/dd[3]" },
                    { "Spectators", "/html/body/div[4]/table/tbody/tr/td[2]/div/dl/dd[5]" }
                };

                SummaryElementsToLookoutFor_CurrentGame = new List<string>() {
                    "server name", "game type", "map"
                };
                SummaryElementsToLookoutFor_Rules = new List<string>() {
                    "difficulty", "wave", "players", "spectators"
                };

                LogEngine.Log(Utils.Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded ServerInfo page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch(NoSuchElementException nsee) {
                LogEngine.Log(Utils.Status.PAGELOAD_FAILURE, "Failed to load ServerInfo page");
                return new Tuple<bool, string>(false, nsee.Message);
            }
        }


        public Tuple<bool, Dictionary<string, string>> Status() {

            /// Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            Driver.Navigate().Refresh();

            Dictionary<string, string> tmpSummaryDict = new Dictionary<string, string>();

            string SectionXPath = "/html/body/div[4]/table/tbody/tr/td[{0}]/div/dl/{1}[{2}]";
            string dt = string.Empty;
            int row = 1;

            /// Retrieve each value as identified in Current Game section via XPaths
            while (true) {
 
                try {
                    dt = Driver.FindElement(By.XPath(string.Format(SectionXPath, 1, "dt", row))).Text;
                } catch (NoSuchElementException) { break; }

                if (SummaryElementsToLookoutFor_CurrentGame.Contains(dt.ToLower())) {
                    tmpSummaryDict.Add(dt, Driver.FindElement(By.XPath(
                        string.Format(SectionXPath, 1, "dd", row))).Text);
                }

                row++;
            }

            row = 1;

            /// Retrieve each value as identified in Rules section via XPaths
            while (true) {

                try {
                    dt = Driver.FindElement(By.XPath(string.Format(SectionXPath, 2, "dt", row))).Text;
                } catch (NoSuchElementException) { break; }

                if (SummaryElementsToLookoutFor_Rules.Contains(dt.ToLower())) {
                    tmpSummaryDict.Add(dt, Driver.FindElement(By.XPath(
                        string.Format(SectionXPath, 2, "dd", row))).Text);
                }

                row++;
            }

            return new Tuple<bool, Dictionary<string, string>>(true, tmpSummaryDict);

        }


    }

}
