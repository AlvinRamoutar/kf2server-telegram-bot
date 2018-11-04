using kf2server_tbot_client.ServerAdmin.CurrentGame;
using kf2server_tbot_client.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace kf2server_tbot_client.ServerAdmin.Settings {

    class General : WebminPage {

        #region Singleton Structure
        private static General instance = null;
        private static readonly object padlock = new object();

        public static General Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new General();
                    }
                    return instance;
                }
            }
        }

        General() { }
        #endregion

        #region Properties
        /// Dictionary of saved GameDifficulties Select element options
        /// Until further notice from TripWire, will be keeping key as string to handle their 'unique' select DOM values
        private Dictionary<string, string> GameDifficulties { get; set; }

        /// Dictionary of saved GameLengths Select element options
        private Dictionary<int, string> GameLengths { get; set; }
        #endregion

        public override Tuple<bool, string> Init() {

            try {

                WindowHandleID = OpenPage(Properties.Settings.Default.GeneralURL, "//*[@id=\"chatwindowframe\"]");

                PageManager.Pages[PageType.General] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

                /// Grab possible Difficulties from dropdown
                GameDifficulties = new Dictionary<string, string>();
                SelectElement gameDifficultySelect = new SelectElement(Driver.FindElement(By.Id("settings_GameDifficulty")));
                foreach (IWebElement option in gameDifficultySelect.Options) {
                    GameDifficulties.Add(option.GetAttribute("value"), option.Text.Trim().ToLower().Replace(" ", string.Empty));

                }

                /// Grab possible Lengths from dropdown
                GameLengths = new Dictionary<int, string>();
                SelectElement gameLengthSelect = new SelectElement(Driver.FindElement(By.Name("settings_GameLength")));
                foreach (IWebElement option in gameLengthSelect.Options) {
                    GameLengths.Add(Convert.ToInt32(option.GetAttribute("value")), option.Text.Trim().ToLower().Replace(" ", string.Empty));

                }

                LogEngine.Log(Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded General page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch (NoSuchElementException nsee) {
                LogEngine.Log(Status.PAGELOAD_FAILURE, "Failed to load General page");
                return new Tuple<bool, string>(false, nsee.Message);
            }
        }



        public Tuple<bool, string> ChangeGameDifficulty(string rawDifficulty) {

            try {

                string difficulty = rawDifficulty.Trim().ToLower().Replace(" ", string.Empty);

                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                double difficultyValue = 0f;

                if (double.TryParse(difficulty, out difficultyValue) &&
                    GameDifficulties.ContainsKey(string.Format("{0:0.0000}", difficultyValue))) {

                    new SelectElement(Driver.FindElement(By.Name("settings_GameDifficulty")))
                        .SelectByValue(difficultyValue.ToString());
                    Driver.FindElement(By.Id("btnsave")).Click();

                } else if (GameDifficulties.ContainsValue(difficulty)) {

                    new SelectElement(Driver.FindElement(By.Name("settings_GameDifficulty")))
                        /// Get key from value (text)
                        .SelectByValue(GameDifficulties.FirstOrDefault(x => x.Value == difficulty).Key.ToString());
                    Driver.FindElement(By.Id("btnsave")).Click();

                } else {
                    return new Tuple<bool, string>(false,
                        string.Format("Game Difficulty '{0}' does not exist", difficulty));
                }


                /// Trigger map change to apply settings instantly to new game session
                return ApplySettingsTrigger();

            } catch(Exception e) {
                LogEngine.Log(Status.SERVICE_INFO, string.Format("Unknown error with ChangeGameDifficulty ({0})", e.Message));

                return new Tuple<bool, string>(false, string.Format("Unknown error with ChangeGameDifficulty ({0})", e.Message));
            }
        }



        public Tuple<bool, string> ChangeGameLength(string rawLength) {

            try {

                string length = rawLength.Trim().ToLower().Replace(" ", string.Empty);

                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                int lengthValue = 0;

                if (int.TryParse(length, out lengthValue) &&
                    GameLengths.ContainsKey(lengthValue)) {

                    new SelectElement(Driver.FindElement(By.Name("settings_GameLength")))
                        .SelectByValue(lengthValue.ToString());
                    Driver.FindElement(By.Id("btnsave")).Click();

                } else if (GameLengths.ContainsValue(length)) {

                    new SelectElement(Driver.FindElement(By.Name("settings_GameLength")))
                        /// Get key from value (text)
                        .SelectByValue(GameLengths.FirstOrDefault(x => x.Value == length).Key.ToString());
                    Driver.FindElement(By.Id("btnsave")).Click();

                } else {
                    return new Tuple<bool, string>(false, "Game Length does not exist");
                }



                /// Trigger map change to apply settings instantly to new game session
                return ApplySettingsTrigger(); 

            } catch (Exception e) {
                LogEngine.Log(Status.SERVICE_INFO, string.Format("Unknown error with ChangeGameLength ({0})", e.Message));

                return new Tuple<bool, string>(false, string.Format("Unknown error with ChangeGameLength ({0})", e.Message));
            }
        }



        public Tuple<bool, string> ChangeGameDifficultyAndLength(string rawDifficulty, string rawLength) {

            try { 

                string difficulty = rawDifficulty.Trim().ToLower().Replace(" ", string.Empty);
                string length = rawLength.Trim().ToLower().Replace(" ", string.Empty);


                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                double difficultyValue = 0f;
                int lengthValue = 0;

                /// Difficulty
                if (double.TryParse(difficulty, out difficultyValue) &&
                    GameDifficulties.ContainsKey(string.Format("{0:0.0000}", difficultyValue))) {

                    new SelectElement(Driver.FindElement(By.Name("settings_GameDifficulty")))
                        .SelectByValue(difficultyValue.ToString());

                } else if (GameDifficulties.ContainsValue(difficulty)) {

                    new SelectElement(Driver.FindElement(By.Name("settings_GameDifficulty")))
                        /// Get key from value (text)
                        .SelectByValue(GameDifficulties.FirstOrDefault(x => x.Value == difficulty).Key.ToString());

                } else {
                    return new Tuple<bool, string>(false,
                        string.Format("Game Difficulty '{0}' does not exist", difficulty));
                }

                /// Length
                if (int.TryParse(length, out lengthValue) &&
                    GameLengths.ContainsKey(lengthValue)) {

                    new SelectElement(Driver.FindElement(By.Name("settings_GameLength")))
                        .SelectByValue(lengthValue.ToString());

                } else if (GameLengths.ContainsValue(length)) {

                    new SelectElement(Driver.FindElement(By.Name("settings_GameLength")))
                    /// Get key from value (text)
                    .SelectByValue(GameLengths.FirstOrDefault(x => x.Value == length).Key.ToString());

                } else {
                    return new Tuple<bool, string>(false,
                        string.Format("Game Length '{0}' does not exist", length));
                }

                Driver.FindElement(By.Id("btnsave")).Click();


                /// Trigger map change to apply settings instantly to new game session
                return ApplySettingsTrigger();

            } catch (Exception e) {
                LogEngine.Log(Status.SERVICE_INFO, string.Format("Unknown error with ChangeGameDifficultyAndLength ({0})", e.Message));

                return new Tuple<bool, string>(false, string.Format("Unknown error with ChangeGameDifficultyAndLength ({0})", e.Message));
            }
        }


        private Tuple<bool, string> ApplySettingsTrigger() {

            if (ChangeMap.Instance.TriggerMapChange()) {
                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                /// Navigates BACK to this page
                Driver.Navigate().GoToUrl(Properties.Settings.Default.KF2ServerURL +
                    Properties.Settings.Default.GeneralURL);

                return new Tuple<bool, string>(true, null);

            } else {
                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                /// Navigates BACK to this page
                Driver.Navigate().GoToUrl(Properties.Settings.Default.KF2ServerURL +
                    Properties.Settings.Default.GeneralURL);

                return new Tuple<bool, string>(false, "Problem when triggering map change");
            }

        }


    }
}
