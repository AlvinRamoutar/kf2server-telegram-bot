using LogEngine;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.ServerAdmin.CurrentGame {


    /// <summary>
    /// Current Game / Players page
    /// </summary>
    class Players : WebminPage{

        #region Singleton Structure
        private static Players instance = null;
        private static readonly object padlock = new object();

        public static Players Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new Players();
                    }
                    return instance;
                }
            }
        }

        Players() { }
        #endregion

        public override Tuple<bool, string> Init() {

            try {
                OpenPage(Properties.Settings.Default.PlayersURL, "//*[@id=\"chatwindowframe\"]");

                PageManager.Pages[PageType.Players] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

                Logger.Log(Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded Players page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch(NoSuchElementException nsee) {
                Logger.Log(Status.PAGELOAD_FAILURE, "Failed to load Players page");
                return new Tuple<bool, string>(false, nsee.Message);
            }
            
        }

        /// <summary>
        /// Retrieves player names column values, concatenates into string 
        /// </summary>
        /// <returns>Tuple result with successful flag, and 'online players' string</returns>
        public Tuple<bool, string> Online() {

            /// Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            Driver.Navigate().Refresh();

            HashSet<string> tmpPlayersList = new HashSet<string>();
            string tmpPlayersListString = string.Empty;

            string playernameXPath = "/html/body/div[4]/div/table/tbody/tr[{0}]/td[2]";


            /// Retrieve each player name
            int playerCounter = 1;
            try {
                while (true) {
                    tmpPlayersList.Add(Driver.FindElement(By.XPath(
                        string.Format(playernameXPath, playerCounter))).Text);

                    playerCounter++;
                }
            } catch (Exception) { }


            /// Create comma-delimited string
            playerCounter = 0;
            foreach(string p in tmpPlayersList) {
                tmpPlayersListString += p + ", ";
                playerCounter++;
            }


            if(playerCounter == 0) {
                return new Tuple<bool, string>(true, "No one is currently online.");
            } else {
                return new Tuple<bool, string>(true,
                     (tmpPlayersListString.IndexOf(',') != -1) ? tmpPlayersListString.Substring(0, tmpPlayersListString.Length - 2) : "");
            }
        }


        /// <summary>
        /// Kicks a player from active game session
        /// </summary>
        /// <param name="playername">In-game name of player to kick</param>
        /// <returns>Tuple result</returns>
        public Tuple<bool, string> Kick(string playername) {

            /// Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            Driver.Navigate().Refresh();

            string playernameXPath = "/html/body/div[4]/div/table/tbody/tr[{0}]/td[2]";
            string formSelectXPath = "/html/body/div[4]/div/table/tbody/tr[{0}]/td[10]/form/div/select";
            string formPlayerName = string.Empty;
            int playerCounter = 1;

            /// Retrieve that particular player's action form
            try {

                while (true) {
                    /// Try to retrieve playername of current row
                    formPlayerName = Driver.FindElement(By.XPath(
                        string.Format(playernameXPath, playerCounter))).Text;


                    /// If player exists on this row
                    if (formPlayerName.Equals(playername)) {
                        new SelectElement(Driver.FindElement(By.XPath(
                            string.Format(formSelectXPath, playerCounter)))).SelectByValue("kick");

                        Driver.FindElement(By.XPath(
                            string.Format(formSelectXPath.Replace("select", "button"), playerCounter))).Click();

                        break;
                    }

                    playerCounter++;
                }

            } catch(NoSuchElementException) {
                /// Player doesn't exist in list of online players
                return new Tuple<bool, string>(false, null);
            }


            return new Tuple<bool, string>(true, null);

        }

    }

}
