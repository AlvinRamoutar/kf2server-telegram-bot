using kf2server_tbot.ServerAdmin.AccessPolicy;
using kf2server_tbot.ServerAdmin.CurrentGame;
using kf2server_tbot.ServerAdmin.Settings;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.ServerAdmin {


    /// <summary>
    /// Known, navigateable pages
    /// </summary>
    public enum PageType {
        Login,
        ServerInfo, ChangeMap, Players,
        Passwords,
        General
    }


    /// <summary>
    /// Handles launching of windows for each navegateable page using FirefoxDriver
    /// </summary>
    class PageManager {

        #region Singleton Structure
        private static PageManager instance = null;
        private static readonly object padlock = new object();

        public static PageManager Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new PageManager();
                    }
                    return instance;
                }
            }
        }
        PageManager() { }
        #endregion

        #region Properties and Fields
        IWebDriver Driver { get; set; }

        public Login Login { get; set; }
        public ChatConsole ChatConsole { get; set; }
        public ServerInfo ServerInfo { get; set; }
        public ChangeMap ChangeMap { get; set; }
        public Players Players { get; set; }
        public Passwords Passwords { get; set; }
        public General General { get; set; }

        public static Dictionary<PageType, string> Pages = new Dictionary<PageType, string>();
        #endregion


        /// <summary>
        /// Launches a FirefoxDriver browser window for each navigateable page
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public Tuple<bool, string> Init(IWebDriver driver) {

            string errorMessage = null;

            Login = Login.Instance;
            Login.Driver = driver;
            Login.SignIn();

            ChatConsole = ChatConsole.Instance;
            ChatConsole.Driver = driver;

            ServerInfo = ServerInfo.Instance;
            ServerInfo.Driver = driver;
            ServerInfo.Init();

            ChangeMap = ChangeMap.Instance;
            ChangeMap.Driver = driver;
            ChangeMap.Init();

            Players = Players.Instance;
            Players.Driver = driver;
            Players.Init();

            Passwords = Passwords.Instance;
            Passwords.Driver = driver;
            Passwords.Init();

            General = General.Instance;
            General.Driver = driver;
            General.Init();


            return new Tuple<bool, string>(true, errorMessage);
        }

    }

}
