using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace kf2server_tbot_client.ServerAdmin {

    abstract class WebminPage {

        #region Properties
        public IWebDriver Driver { get; set; }

        public string WindowHandleID { get; set; }

        bool InProgress { get; set; }
        #endregion

        /// <summary>
        /// Launches a new browser window with a unique WindowHandlerID, and browses to ServerAdmin resource at supplied URL.
        /// </summary>
        /// <param name="URL">URL of Page to load in New Window</param>
        /// <returns>WindowHandlerID</returns>
        public string OpenPage(string URL) {

            /// Removes '/' from end of resource URL
            string tmpResourceUrl = URL;
            tmpResourceUrl = (URL.EndsWith("/")) ? URL.Remove(URL.Length - 1) : URL;

            /// Removes '/' from end of server URL
            string tmpServerUrl = Properties.Settings.Default.KF2ServerURL;
            tmpServerUrl = (tmpServerUrl.EndsWith("/")) ? tmpServerUrl : tmpServerUrl + "/" ;

            /// Uses javascript to launch a new window via window.open();
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.open('" + tmpServerUrl + tmpResourceUrl + "', '_blank');");

            /// Switch focus to newly opened window
            Driver.SwitchTo().Window(Driver.WindowHandles.Last());

            return Driver.WindowHandles.Last();
        }


        /// <summary>
        /// Extension method for OpenPage, which waits until the page is loaded.
        /// <para>Page is deemed loaded when element with suppplied XPath is found.</para>
        /// </summary>
        /// <param name="URL">URL of Page to load in New Window</param>
        /// <param name="xPath">XPath of IWebElement. Will wait until this is loaded, or timeout.</param>
        /// <returns>WindowHandlerID</returns>
        public string OpenPage(string URL, string xPath) {

            /// Perform page nav
            string WindowHandleId = OpenPage(URL);

            /// Wait until element is loaded, or timeout as defined in settings.
            new WebDriverWait(Driver, TimeSpan.FromSeconds(Properties.Settings.Default.PageNavTimeoutSeconds))
                .Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists((By.XPath(xPath))));

            return WindowHandleID;
        }


        /// <summary>
        /// Initialization of Resource Page
        /// </summary>
        /// <returns>Result</returns>
        public abstract Tuple<bool, string> Init();
    }

}
