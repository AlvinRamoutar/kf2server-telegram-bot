using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kf2server_tbot_client.ServerAdmin {

    abstract class WebminPage {

        public IWebDriver Driver { get; set; }

        public string WindowHandleID { get; set; }

        bool InProgress { get; set; }



        public void OpenPage(string URL) {

            string tmpResourceUrl = URL;
            tmpResourceUrl = (URL.EndsWith("/")) ? URL.Remove(URL.Length - 1) : URL;

            string tmpServerUrl = Properties.Settings.Default.KF2ServerURL;
            tmpServerUrl = (Properties.Settings.Default.KF2ServerURL.EndsWith("/")) ?
            tmpServerUrl : tmpServerUrl + "/" ;

            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.open('" + tmpServerUrl + tmpResourceUrl + "', '_blank');");

        }

        public abstract Tuple<bool, string> Init();
    }

}
