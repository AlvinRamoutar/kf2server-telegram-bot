using LogEngine;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.ServerAdmin {

    /// <summary>
    /// Login page 
    /// </summary>
    class Login : WebminPage {

        #region Singleton Structure
        private static Login instance = null;
        private static readonly object padlock = new object();

        public static Login Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new Login();
                    }
                    return instance;
                }
            }
        }
        Login() { }
        #endregion

        #region Properties and Fields
        public bool IsLoggedIn { get; set; }
        #endregion


        public override Tuple<bool, string> Init() {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Signs into ServerAdmin using credentials stored in Settings file.
        /// This MUST be done as part of setup
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, string> SignIn() {

            if(IsLoggedIn) {
                return new Tuple<bool, string>(true, "Already logged in");
            }


            string errorMessage = string.Empty;

            try {

                PageManager.Pages[PageType.Login] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

                Driver.FindElement(By.Id("username")).SendKeys(Properties.Settings.Default.WebAdminUsername);

                Driver.FindElement(By.Id("password")).SendKeys(Properties.Settings.Default.WebAdminPassword);

                SelectElement rememberDOM = new SelectElement(Driver.FindElement(By.Name("remember")));

                rememberDOM.SelectByValue("2678400");

                Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                /// Check if there exists an error message. If there is, then login failed.
                /// Else, exception is thrown, which means a successful login.
                try { 
                    errorMessage = Driver.FindElement(By.CssSelector("div[class='message error']")).Text;

                    Logger.Log(Status.PAGELOAD_FAILURE, string.Format("Failed to login: {0}", errorMessage));
                    IsLoggedIn = false;
                    return new Tuple<bool, string>(false, string.Format("Failed to login: {0}", errorMessage));

                } catch(NoSuchElementException) {
                    Logger.Log(Status.PAGELOAD_SUCCESS, string.Format("Successfully logged in ({0})", WindowHandleID));
                    IsLoggedIn = true;
                }


            } catch(NoSuchElementException nsee) {
                return new Tuple<bool, string>(false, nsee.Message);
            }

            return new Tuple<bool, string>(true, null);
        }

    }

}
