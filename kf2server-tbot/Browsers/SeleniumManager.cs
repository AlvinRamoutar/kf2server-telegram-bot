using kf2server_tbot.ServerAdmin;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.Browsers {

    /// <summary>
    /// Manages the Selenium WebDriver (FirefoxDriver)
    /// </summary>
    class SeleniumManager {

        #region Properties and Fields
        PageManager PageManager { get; set; }

        public FirefoxProfile Profile { get; set; }
        public FirefoxOptions Options { get; set; }

        public IWebDriver Driver { get; set; }

        public static IWebDriver CurrentDriver { get; private set; }
        #endregion

        /// <summary>
        /// Constructs new selenium driver using FirefoxDriver.
        /// <para>Also assigns profile options to FirefoxDriver, such as headless and no img/css rendering (via permission block)</para>
        /// </summary>
        public SeleniumManager() {

            Profile = new FirefoxProfile();

            /// Disable CSS and images to save rendering resources
            Profile.SetPreference("permissions.default.stylesheet", 2);
            Profile.SetPreference("permissions.default.image", 2);

            Options = new FirefoxOptions();
            Options.AddArguments("--headless");
            Options.Profile = Profile;

            Driver = new FirefoxDriver(Options);
            Driver.Url = Properties.Settings.Default.KF2ServerURL;

            CurrentDriver = Driver;

            /// Prepares appropriate pages from left sidebar, each in its own window
            PageManager = PageManager.Instance;
            PageManager.Init(Driver);

        }


        /// <summary>
        /// Terminate Selenium Driver, and close all open browsers.
        /// </summary>
        /// <returns>Tuple with success flag and message</returns>
        public static Tuple<bool, string> Quit() {

            try {
                /// Try to close all open windows from FirefoxDriver, since calling
                ///  WebDriver.Quit() has a bad tendency to leave marionettes open...
                foreach (string id in CurrentDriver.WindowHandles) {
                    CurrentDriver.SwitchTo().Window(id);
                    CurrentDriver.Close();
                }

                CurrentDriver.Quit();

            } catch (Exception e) {
                return new Tuple<bool, string>(false, e.Message);
            }

            return new Tuple<bool, string>(true, null);
        }

    }

}
