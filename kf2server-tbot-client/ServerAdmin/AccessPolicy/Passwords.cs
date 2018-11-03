using kf2server_tbot_client.Utils;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override Tuple<bool, string> Init() {

            try {
                OpenPage(Properties.Settings.Default.PasswordsURL, "//*[@id=\"chatwindowframe\"]");

                PageManager.Pages[PageType.Passwords] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

                LogEngine.Log(Status.PAGELOAD_SUCCESS, string.Format("Successfully loaded Passwords page ({0})", WindowHandleID));

                return new Tuple<bool, string>(true, null);

            } catch(NoSuchElementException nsee) {
                LogEngine.Log(Status.PAGELOAD_FAILURE, "Failed to load Passwords page");
                return new Tuple<bool, string>(false, nsee.Message);
            }

            
        }


        /// <summary>
        /// Sets the game password to value supplied.
        /// If none supplied, game password is removed.
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public Tuple<bool, string> GamePwd(string pwd = null) {

            /// Changes focus to this page
            Driver.SwitchTo().Window(WindowHandleID);

            IWebElement GamePasswordForm = Driver.FindElement(By.Id("gamepassword"));

            if(!string.IsNullOrEmpty(pwd)) {
                GamePasswordForm.FindElement(By.Name("gamepw1")).SendKeys(pwd);
                GamePasswordForm.FindElement(By.Name("gamepw2")).SendKeys(pwd);
            }

            GamePasswordForm.FindElement(By.CssSelector("input[type='submit']"));

            return new Tuple<bool, string>(true, null);

        }
    }

}
