﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

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
        // Dictionary of saved GameDifficulties Select element options
        private Dictionary<double, string> GameDifficulties { get; set; }

        // Dictionary of saved GameLengths Select element options
        private Dictionary<int, string> GameLengths { get; set; }
        #endregion

        public override Tuple<bool, string> Init() {
            
            WindowHandleID = OpenPage(Properties.Settings.Default.GeneralURL, "//*[@id=\"chatwindowframe\"]");
            PageManager.Pages[PageType.General] = WindowHandleID;

            // Grab possible Difficulties from dropdown
            GameDifficulties = new Dictionary<double, string>();
            SelectElement gameDifficultySelect = new SelectElement(Driver.FindElement(By.Id("settings_GameDifficulty")));
            foreach (IWebElement option in gameDifficultySelect.Options) {
                GameDifficulties.Add(Convert.ToDouble(option.GetAttribute("value")), option.Text);
            }

            // Grab possible Lengths from dropdown
            GameLengths = new Dictionary<int, string>();
            SelectElement gameLengthSelect = new SelectElement(Driver.FindElement(By.Name("settings_GameLength")));
            foreach (IWebElement option in gameLengthSelect.Options) {
                GameLengths.Add(Convert.ToInt32(option.GetAttribute("value")), option.Text);
            }

            return new Tuple<bool, string>(true, null);
        }



        public Tuple<bool, string> ChangeGameDifficulty(string difficulty) {

            // Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            double difficultyValue = 0f;

            if (double.TryParse(difficulty, out difficultyValue) &&
                GameDifficulties.ContainsKey(difficultyValue)) {

                new SelectElement(Driver.FindElement(By.Name("settings_GameDifficulty")))
                    .SelectByValue(difficultyValue.ToString());
                Driver.FindElement(By.Id("btnsave")).Click();

            } else if (GameDifficulties.ContainsValue(difficulty)) {

                new SelectElement(Driver.FindElement(By.Name("settings_GameDifficulty")))
                    .SelectByText(difficultyValue.ToString());
                Driver.FindElement(By.Id("btnsave")).Click();

            } else {
                return new Tuple<bool, string>(false, 
                    string.Format("Game Difficulty '{0}' does not exist", difficulty));
            }


            return new Tuple<bool, string>(true, null);
        }



        public Tuple<bool, string> ChangeGameLength(string length) {

            // Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            int lengthValue = 0;

            if (int.TryParse(length, out lengthValue) &&
                GameDifficulties.ContainsKey(lengthValue)) {

                new SelectElement(Driver.FindElement(By.Name("settings_GameLength")))
                    .SelectByValue(lengthValue.ToString());
                Driver.FindElement(By.Id("btnsave")).Click();

            } else if (GameDifficulties.ContainsValue(length)) {

                new SelectElement(Driver.FindElement(By.Name("settings_GameLength")))
                    .SelectByText(length);
                Driver.FindElement(By.Id("btnsave")).Click();

            } else {
                return new Tuple<bool, string>(false, "Game Length does not exist");
            }


            return new Tuple<bool, string>(true, null);

        }



        public Tuple<bool, string> ChangeGameDifficultyAndLength(string difficulty, string length) {

            // Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            double difficultyValue = 0f;
            int lengthValue = 0;

            // Difficulty
            if (double.TryParse(difficulty, out difficultyValue) &&
                GameDifficulties.ContainsKey(difficultyValue)) {

                new SelectElement(Driver.FindElement(By.Name("settings_GameDifficulty")))
                    .SelectByValue(difficultyValue.ToString());

            } else if (GameDifficulties.ContainsValue(difficulty)) {

                new SelectElement(Driver.FindElement(By.Name("settings_GameDifficulty")))
                    .SelectByText(difficultyValue.ToString());

            } else {
                return new Tuple<bool, string>(false,
                    string.Format("Game Difficulty '{0}' does not exist", difficulty));
            }

            // Length
            if (int.TryParse(length, out lengthValue) &&
                GameDifficulties.ContainsKey(lengthValue)) {

                new SelectElement(Driver.FindElement(By.Name("settings_GameLength")))
                    .SelectByValue(lengthValue.ToString());

            } else if (GameDifficulties.ContainsValue(length)) {

                new SelectElement(Driver.FindElement(By.Name("settings_GameLength")))
                    .SelectByText(length);

            } else {
                return new Tuple<bool, string>(false, 
                    string.Format("Game Length '{0}' does not exist", length));
            }

            Driver.FindElement(By.Id("btnsave")).Click();
            return new Tuple<bool, string>(true, null);

        }
    }
}