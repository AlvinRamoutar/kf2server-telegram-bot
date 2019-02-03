using LogEngine;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.ServerAdmin.CurrentGame {


    /// <summary>
    /// ChangeMap page
    /// </summary>
    class ChangeMap : WebminPage{

        #region Singleton Structure
        private static ChangeMap instance = null;
        private static readonly object padlock = new object();

        public static ChangeMap Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new ChangeMap();
                    }
                    return instance;
                }
            }
        }

        ChangeMap() { }
        #endregion

        #region Properties and Fields
        private Dictionary<string, string> GameType { get; set; }
        private Dictionary<string, string> Maps { get; set; }
        #endregion


        /// <summary>
        /// Creates a collection from gametypes dropdown and maps dropdown.
        /// <para>This collection is used to ensure a valid, known gametype/map is chosen.</para>
        /// </summary>
        /// <returns>Tuple result</returns>
        public override Tuple<bool, string> Init() {

            try {

                OpenPage(Properties.Settings.Default.ChangeMapURL, "//*[@id=\"chatwindowframe\"]");

                PageManager.Pages[PageType.ChangeMap] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];


                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                /// Waits until map select dropdown loads. Assuming once loaded, both dropdowns are loaded.
                new WebDriverWait(Driver, TimeSpan.FromSeconds(10)).Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists((By.Id("map"))));

                /// Grab possible GameTypes from dropdown
                GameType = new Dictionary<string, string>();
                SelectElement gameTypesSelect = new SelectElement(Driver.FindElement(By.Name("gametype")));
                foreach (IWebElement option in gameTypesSelect.Options) {
                    GameType.Add(option.GetAttribute("value"), option.Text);
                }

                /// Grab possible Maps from dropdown
                Maps = new Dictionary<string, string>();
                SelectElement mapsSelect = new SelectElement(Driver.FindElement(By.Name("map")));
                foreach (IWebElement option in mapsSelect.Options) {
                    Maps.Add(option.GetAttribute("value"), option.Text);
                }

                Logger.Log(Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded ChangeMap page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch(NoSuchElementException nsee) {
                Logger.Log(Status.PAGELOAD_FAILURE, "Failed to load ChangeMap page");
                return new Tuple<bool, string>(false, nsee.Message);
            }
        }



        /// <summary>
        /// Changes gametype, then performs a change map to apply to new game session.
        /// </summary>
        /// <param name="gametype">Gametype</param>
        /// <returns>Tuple result</returns>
        public Tuple<bool, string> ChangeGameTypeOnly(string gametype) {

            /// Check if gametype even exist
            if (!GameType.ContainsKey(gametype))
                return new Tuple<bool, string>(false, "Gametype does not exist.");

            /// Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            /// Save current map, since changing game type resets map to first map in select dropdown
            string currentMap = new SelectElement(Driver.FindElement(By.Name("map"))).SelectedOption.Text;

            /// Perform game type selection
            new SelectElement(Driver.FindElement(By.Name("gametype"))).SelectByValue(gametype);

            /// Perform map re-selection
            new SelectElement(Driver.FindElement(By.Name("map"))).SelectByValue(currentMap);

            Driver.FindElement(By.Id("btnchange")).Click();

            WebDriverWait waiter = new WebDriverWait(Driver, new TimeSpan(0, 0,
                Properties.Settings.Default.MapChangeTimeoutSeconds));
            waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(
                Properties.Settings.Default.ServerInfoURL));

            Driver.Navigate().GoToUrl(Properties.Settings.Default.KF2ServerURL +
                Properties.Settings.Default.ChangeMapURL);

            return new Tuple<bool, string>(true, null);
        }


        /// <summary>
        /// Changes map, then performs a change map to apply to new game session.
        /// </summary>
        /// <param name="map">Map name (e.g. KF2-*)</param>
        /// <returns>Tuple result</returns>
        public Tuple<bool, string> ChangeMapOnly(string map) {

            string mapToChangeTo = null;
            foreach (KeyValuePair<string, string> m in Maps) {
                if (m.Key.ToLower().Contains(map)) {
                    mapToChangeTo = m.Key;
                    break;
                }
            }

            /// Check if map even exist
            if(mapToChangeTo == null)
                return new Tuple<bool, string>(false, "Map does not exist.");


            /// Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            new SelectElement(Driver.FindElement(By.Name("map"))).SelectByValue(mapToChangeTo);

            Driver.FindElement(By.Id("btnchange")).Click();

            WebDriverWait waiter = new WebDriverWait(Driver, TimeSpan.FromSeconds(
                Properties.Settings.Default.MapChangeTimeoutSeconds));
            waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(
                Properties.Settings.Default.ServerInfoURL));

            Driver.Navigate().GoToUrl(Properties.Settings.Default.KF2ServerURL +
                Properties.Settings.Default.ChangeMapURL);

            return new Tuple<bool, string>(true, null);

        }


        /// <summary>
        /// Changes both gametype and map, then performs a change map to apply to new game session.
        /// </summary>
        /// <param name="gametype">Gametype</param>
        /// <param name="map">Map name (e.g. KF2-*)</param>
        /// <returns>Tuple result</returns>
        public Tuple<bool, string> ChangeMapAndGameType(string gametype, string map) {

            string gameTypeToChangeTo = null;
            foreach (KeyValuePair<string, string> gt in GameType) {
                if (gt.Key.ToLower().Contains(gametype)) {
                    gameTypeToChangeTo = gt.Key;
                    break;
                }
            }

            string mapToChangeTo = null;
            foreach (KeyValuePair<string, string> m in Maps) {
                if (m.Key.ToLower().Contains(map)) {
                    mapToChangeTo = m.Key;
                    break;
                }
            }

            /// Check if gametype & map even exist
            if (gameTypeToChangeTo == null || mapToChangeTo == null)
                return new Tuple<bool, string>(false, "Gametype/Map does not exist.");

            /// Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            new SelectElement(Driver.FindElement(By.Name("gametype"))).SelectByValue(gameTypeToChangeTo);
            new SelectElement(Driver.FindElement(By.Name("map"))).SelectByValue(mapToChangeTo);

            Driver.FindElement(By.Id("btnchange")).Click();

            WebDriverWait waiter = new WebDriverWait(Driver, new TimeSpan(0, 0, 
                Properties.Settings.Default.MapChangeTimeoutSeconds));
            waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(
                Properties.Settings.Default.ServerInfoURL));

            Driver.Navigate().GoToUrl(Properties.Settings.Default.KF2ServerURL + 
                Properties.Settings.Default.ChangeMapURL);

            return new Tuple<bool, string>(true, null);
        }



        /// <summary>
        /// Triggers a map change to force certain game session settings (e.g. difficulty, length changes)
        /// </summary>
        /// <returns>True if successful, else false.</returns>
        public bool TriggerMapChange() {

            try {
                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                Driver.FindElement(By.Id("btnchange")).Click();

                WebDriverWait waiter = new WebDriverWait(Driver, new TimeSpan(0, 0,
                    Properties.Settings.Default.MapChangeTimeoutSeconds));
                waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(
                    Properties.Settings.Default.ServerInfoURL));

                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                /// Navigates BACK to this page
                Driver.Navigate().GoToUrl(Properties.Settings.Default.KF2ServerURL +
                    Properties.Settings.Default.ChangeMapURL);

                return true;

            } catch (Exception) {
                return false;
            }

        }

    }

}
