using kf2server_tbot_client.ServerAdmin;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kf2server_tbot_client.Browsers {

    class SeleniumManager {

        PageManager PageManager { get; set; }

        public FirefoxProfile Profile { get; set; }
        public FirefoxOptions Options { get; set; }
        public IWebDriver Driver { get; set; }

        public SeleniumManager() {

            Profile = new FirefoxProfile();
            Profile.SetPreference("permissions.default.stylesheet", 2);
            Profile.SetPreference("permissions.default.image", 2);

            Options = new FirefoxOptions();
            //Options.AddArguments("--headless");
            Options.Profile = Profile;

            Driver = new FirefoxDriver(Options);
            Driver.Url = Properties.Settings.Default.KF2ServerURL;

            PageManager = PageManager.Instance;
            PageManager.Init(Driver);

        }


        public Tuple<bool, string> Quit() {

            try {
                Driver.Quit();
            } catch(Exception e) {
                return new Tuple<bool, string>(false, e.Message);
            }

            return new Tuple<bool, string>(true, null);
        }

    }

}
