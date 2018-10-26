using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kf2server_tbot_client.ServerAdmin {

    class ChatConsole : WebminPage {

        #region Singleton Structure
        private static ChatConsole instance = null;
        private static readonly object padlock = new object();

        public static ChatConsole Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new ChatConsole();
                    }
                    return instance;
                }
            }
        }

        ChatConsole() { }
        #endregion
    

        public override Tuple<bool, string> Init() {
            throw new NotImplementedException();
        }


        public static void SendMessage(string msg) {

            IWebElement chatWindowFrame = 
                ChatConsole.Instance.Driver.FindElement(By.XPath("//iframe[@id='chatwindowframe']"));

            ChatConsole.Instance.Driver.SwitchTo().Frame(chatWindowFrame);

            ChatConsole.Instance.Driver.FindElement(
                By.CssSelector("input#chatmessage")).SendKeys(msg);

            ChatConsole.Instance.Driver.FindElement(By.TagName("button")).Click();

            ChatConsole.Instance.Driver.SwitchTo().DefaultContent();

        }


    }
}
