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

            OpenPage(Properties.Settings.Default.PasswordsURL);

            PageManager.Pages[PageType.Passwords] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
            WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

            return new Tuple<bool, string>(true, null);
        }


        /// <summary>
        /// Sets the game password to value supplied.
        /// If none supplied, game password is removed.
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public Tuple<bool, string> GamePwd(string pwd = null) {

            // Changes focus to this page
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
