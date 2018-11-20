using kf2server_tbot_client.Utils;
using LogEngine;
using OpenQA.Selenium;
using System;

namespace kf2server_tbot_client.ServerAdmin.AccessPolicy {

    class Passwords : WebminPage {

        #region Singleton Structure
        private static Passwords instance = null;
        private static readonly object padlock = new object();

        public static Passwords Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new Passwords();
                    }
                    return instance;
                }
            }
        }

        Passwords() { }
        #endregion


        /// <summary>
        /// Initializes Passwords page
        /// </summary>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
        public override Tuple<bool, string> Init() {

            try {
                OpenPage(Properties.Settings.Default.PasswordsURL, "//*[@id=\"chatwindowframe\"]");

                PageManager.Pages[PageType.Passwords] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

                Logger.Log(Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded Passwords page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch(NoSuchElementException nsee) {
                Logger.Log(Status.PAGELOAD_FAILURE, "Failed to load Passwords page");
                return new Tuple<bool, string>(false, nsee.Message);
            }

            
        }


        /// <summary>
        /// Sets the game password to value supplied.
        /// If none supplied, game password is removed.
        /// </summary>
        /// <param name="pwd">Game Password</param>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
        public Tuple<bool, string> GamePwd(string pwd = null) {

            try {

                /// Changes focus to this page
                Driver.SwitchTo().Window(WindowHandleID);

                IWebElement GamePasswordForm = Driver.FindElement(By.Id("gamepassword"));

                if (!string.IsNullOrEmpty(pwd)) {
                    GamePasswordForm.FindElement(By.Name("gamepw1")).SendKeys(pwd);
                    GamePasswordForm.FindElement(By.Name("gamepw2")).SendKeys(pwd);
                }

                GamePasswordForm.FindElement(By.CssSelector("input[type='submit']"));

                Logger.Log(Status.SERVICE_INFO, string.Format("Successfully executed GamePwd ({0})", string.IsNullOrEmpty(pwd) ? "none" : pwd));

                return new Tuple<bool, string>(true, null);

            } catch(Exception e) {
                Logger.Log(Status.SERVICE_INFO, string.Format("Unknown error with GamePwd ({0})", e.Message));

                return new Tuple<bool, string>(false, string.Format("Unknown error with GamePwd ({0})", e.Message));
            }

        }
    }

}
