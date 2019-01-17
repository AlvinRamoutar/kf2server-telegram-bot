using kf2server_tbot.ServerAdmin.CurrentGame;
using LogEngine;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.ServerAdmin.Settings {

    /// <summary>
    /// Handles operations related to ServerAdmin/General#SG_Game
    /// </summary>
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

        /// <summary>
        /// Retrieves possible dropdown options from page, and stores in dictionary.
        /// <para>Reduces the amount of effort required to ensure an option exists.</para>
        /// </summary>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
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

                Logger.Log(Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded General page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch (NoSuchElementException nsee) {
                Logger.Log(Status.PAGELOAD_FAILURE, "Failed to load General page");
                return new Tuple<bool, string>(false, nsee.Message);
            }
        }


        /// <summary>
        /// Performs a change in game difficulty.
        /// <para>Check if difficulty is a double (Key) or string (Value). Then, check if it exists in local dict. 
        ///  Finally, apply to new game session via map change.</para>
        /// </summary>
        /// <param name="rawDifficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
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
                Tuple<bool, string> ApplyResult = ApplySettingsTrigger();

                if (ApplyResult.Item1) {
                    Logger.Log(Status.SERVERADMIN_INFO, string.Format("Successfully executed ChangeGameDifficulty ({0})", difficulty));
                    return new Tuple<bool, string>(true, string.Empty);
                } else {
                    throw new Exception(ApplyResult.Item2);
                }

            } catch(Exception e) {
                Logger.Log(Status.SERVERADMIN_INFO, string.Format("Unknown error with ChangeGameDifficulty ({0})", e.Message));

                return new Tuple<bool, string>(false, string.Format("Unknown error with ChangeGameDifficulty ({0})", e.Message));
            }
        }


        /// <summary>
        /// Performs a change in game length.
        /// <para>Check if length is an int (Key) or string (Value). Then, check if it exists in local dict. 
        ///  Finally, apply to new game session via map change.</para>
        /// </summary>
        /// <param name="rawLength">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
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
                Tuple<bool, string> ApplyResult = ApplySettingsTrigger();

                if (ApplyResult.Item1) {
                    Logger.Log(Status.SERVERADMIN_INFO, string.Format("Successfully executed ChangeGameLength ({0})", length));
                    return new Tuple<bool, string>(true, string.Empty);
                }
                else {
                    throw new Exception(ApplyResult.Item2);
                }

            } catch (Exception e) {
                Logger.Log(Status.SERVERADMIN_INFO, string.Format("Unknown error with ChangeGameLength ({0})", e.Message));

                return new Tuple<bool, string>(false, string.Format("Unknown error with ChangeGameLength ({0})", e.Message));
            }
        }


        /// <summary>
        /// Performs a change in both difficulty and game length.
        /// <para>Combination of ChangeGameDifficulty logic (<see cref="ChangeGameDifficulty(string)"/>) and
        ///  ChangeGameLength logic (<see cref="ChangeGameLength(string)"/>)</para>
        /// </summary>
        /// <param name="rawDifficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <param name="rawLength">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
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
                Tuple<bool, string> ApplyResult = ApplySettingsTrigger();

                if (ApplyResult.Item1) {
                    Logger.Log(Status.SERVERADMIN_INFO, string.Format("Successfully executed ChangeGameDifficultyAndLength ({0}, {1})", difficulty, length));
                    return new Tuple<bool, string>(true, string.Empty);
                }
                else {
                    throw new Exception(ApplyResult.Item2);
                }

            } catch (Exception e) {
                Logger.Log(Status.SERVERADMIN_INFO, string.Format("Unknown error with ChangeGameDifficultyAndLength ({0})", e.Message));

                return new Tuple<bool, string>(false, string.Format("Unknown error with ChangeGameDifficultyAndLength ({0})", e.Message));
            }
        }


        /// <summary>
        /// Calls TriggerMapChange in ChangeMap, which changes map back to current map.
        /// <para>This creates a new game session, with resulting settings applied</para>
        /// </summary>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
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
