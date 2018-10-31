using kf2server_tbot_client.ServerAdmin;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;

namespace kf2server_tbot_client.Browsers {

    class SeleniumManager {

        #region Properties and Fields
        PageManager PageManager { get; set; }

        public FirefoxProfile Profile { get; set; }
        public FirefoxOptions Options { get; set; }

        public IWebDriver Driver { get; set; }

        public static IWebDriver CurrentDriver { get; private set; }
        #endregion


        public SeleniumManager() {

            Profile = new FirefoxProfile();
            Profile.SetPreference("permissions.default.stylesheet", 2);
            Profile.SetPreference("permissions.default.image", 2);

            Options = new FirefoxOptions();
            //Options.AddArguments("--headless");
            Options.Profile = Profile;

            Driver = new FirefoxDriver(Options);
            Driver.Url = Properties.Settings.Default.KF2ServerURL;

            CurrentDriver = Driver;

            PageManager = PageManager.Instance;
            
            PageManager.Init(Driver);

        }


        public static Tuple<bool, string> Quit() {

            try {

                foreach(string id in CurrentDriver.WindowHandles) {

                    CurrentDriver.SwitchTo().Window(id);
                    CurrentDriver.Close();

                }

                CurrentDriver.Quit();

            } catch(Exception e) {
                return new Tuple<bool, string>(false, e.Message);
            }

            return new Tuple<bool, string>(true, null);
        }

    }

}
