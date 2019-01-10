using LogEngine;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot_client.ServerAdmin.CurrentGame {


    /// <summary>
    /// ServerInfo page
    /// </summary>
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

        #region Properties and Fields
        private List<string> SummaryElementsToLookoutFor_CurrentGame = new List<string>();

        private List<string> SummaryElementsToLookoutFor_Rules = new List<string>();
        #endregion


        /// <summary>
        /// Initializes ServerInfo by assembling a collection of elements to retrieve values from (by dt).
        /// </summary>
        /// <returns></returns>
        public override Tuple<bool, string> Init() {

            try {
                OpenPage(Properties.Settings.Default.ServerInfoURL, "//*[@id=\"chatwindowframe\"]");

                PageManager.Pages[PageType.ServerInfo] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                

                /// Looks for the following dt's in sections '*_$SECTION'.
                SummaryElementsToLookoutFor_CurrentGame = new List<string>() {
                    "server name", "game type", "map"
                };
                SummaryElementsToLookoutFor_Rules = new List<string>() {
                    "difficulty", "wave", "players", "spectators"
                };

                Logger.Log(LogEngine.Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded ServerInfo page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch(NoSuchElementException nsee) {
                Logger.Log(LogEngine.Status.PAGELOAD_FAILURE, "Failed to load ServerInfo page");
                return new Tuple<bool, string>(false, nsee.Message);
            }
        }


        /// <summary>
        /// Retrieves text from page for specified dt's from init
        /// </summary>
        /// <returns></returns>
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
