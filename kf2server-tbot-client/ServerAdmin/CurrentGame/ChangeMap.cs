﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace kf2server_tbot_client.ServerAdmin.CurrentGame {

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

        private Dictionary<string, string> GameType { get; set; }
        private Dictionary<string, string> Maps { get; set; }


        public override Tuple<bool, string> Init() {

            OpenPage(Properties.Settings.Default.ChangeMapURL, "//*[@id=\"chatwindowframe\"]");

            PageManager.Pages[PageType.ChangeMap] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
            WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];


            // Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            // Waits until map select dropdown loads. Assuming once loaded, both dropdowns are loaded.
            new WebDriverWait(Driver, TimeSpan.FromSeconds(10)).Until(
                SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists((By.Id("map"))));

            // Grab possible GameTypes from dropdown
            GameType = new Dictionary<string, string>();
            SelectElement gameTypesSelect = new SelectElement(Driver.FindElement(By.Name("gametype")));
            foreach (IWebElement option in gameTypesSelect.Options) {
                GameType.Add(option.GetAttribute("value"), option.Text);
            }

            // Grab possible Maps from dropdown
            Maps = new Dictionary<string, string>();
            SelectElement mapsSelect = new SelectElement(Driver.FindElement(By.Name("map")));
            foreach (IWebElement option in mapsSelect.Options) {
                Maps.Add(option.GetAttribute("value"), option.Text);
            }

            return new Tuple<bool, string>(true, null);
        }



        public Tuple<bool, string> ChangeGameTypeOnly(string gametype) {

            // Check if gametype even exist
            if (!GameType.ContainsKey(gametype))
                return new Tuple<bool, string>(false, "Gametype does not exist.");

            // Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            new SelectElement(Driver.FindElement(By.Name("gametype"))).SelectByValue(gametype);

            Driver.FindElement(By.Id("btnchange")).Click();

            WebDriverWait waiter = new WebDriverWait(Driver, new TimeSpan(0, 0,
                Properties.Settings.Default.MapChangeTimeoutSeconds));
            waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(
                Properties.Settings.Default.ServerInfoURL));

            Driver.Navigate().GoToUrl(Properties.Settings.Default.KF2ServerURL +
                Properties.Settings.Default.ServerInfoURL);

            return new Tuple<bool, string>(true, null);
        }

        public Tuple<bool, string> ChangeMapOnly(string map) {

            // Check if map even exist
            if (!Maps.ContainsKey(map))
                return new Tuple<bool, string>(false, "Map does not exist.");

            // Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            new SelectElement(Driver.FindElement(By.Name("map"))).SelectByValue(map);

            Driver.FindElement(By.Id("btnchange")).Click();

            WebDriverWait waiter = new WebDriverWait(Driver, TimeSpan.FromSeconds(
                Properties.Settings.Default.MapChangeTimeoutSeconds));
            waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(
                Properties.Settings.Default.ServerInfoURL));

            Driver.Navigate().GoToUrl(Properties.Settings.Default.KF2ServerURL +
                Properties.Settings.Default.ChangeMapURL);

            return new Tuple<bool, string>(true, null);

        }

        public Tuple<bool, string> ChangeMapAndGameType(string gametype, string map) {

            // Check if gametype & map even exist
            if (!GameType.ContainsKey(gametype) || !Maps.ContainsKey(map))
                return new Tuple<bool, string>(false, "Gametype/Map does not exist.");

            // Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            new SelectElement(Driver.FindElement(By.Name("gametype"))).SelectByValue(gametype);
            new SelectElement(Driver.FindElement(By.Name("map"))).SelectByValue(map);

            Driver.FindElement(By.Id("btnchange")).Click();

            WebDriverWait waiter = new WebDriverWait(Driver, new TimeSpan(0, 0, 
                Properties.Settings.Default.MapChangeTimeoutSeconds));
            waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(
                Properties.Settings.Default.ServerInfoURL));

            Driver.Navigate().GoToUrl(Properties.Settings.Default.KF2ServerURL + 
                Properties.Settings.Default.ServerInfoURL);

            return new Tuple<bool, string>(true, null);
        }

    }

}
