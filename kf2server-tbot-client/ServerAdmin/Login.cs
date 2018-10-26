using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;

namespace kf2server_tbot_client.ServerAdmin {

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

        public bool IsLoggedIn { get; set; }

        public override Tuple<bool, string> Init() {
            throw new NotImplementedException();
        }

        public Tuple<bool, string> SignIn() {

            if(IsLoggedIn) {
                return new Tuple<bool, string>(true, Properties.Resources.AlreadyLoggedInMessage);
            }

            try {

                PageManager.Pages[PageType.Login] = Driver.WindowHandles[Driver.WindowHandles.Count - 1];
                WindowHandleID = Driver.WindowHandles[Driver.WindowHandles.Count - 1];

                Driver.FindElement(By.Id("username")).SendKeys("admin");

                Driver.FindElement(By.Id("password")).SendKeys("WelcomeToBrampton69");

                SelectElement rememberDOM = new SelectElement(Driver.FindElement(By.Name("remember")));

                rememberDOM.SelectByValue("2678400");

                Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                string errorMessage = Driver.FindElement(By.CssSelector("div[class='message error']")).Text;

                IsLoggedIn = true;


            } catch(NoSuchElementException nsee) {
                return new Tuple<bool, string>(false, nsee.Message);
            }

            return new Tuple<bool, string>(true, null);
        }

    }

}
