﻿using OpenQA.Selenium;
using System;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.ServerAdmin {


    /// <summary>
    /// ChatConsole frame 
    /// </summary>
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


        /// <summary>
        /// Selects ChatConsole frame, then sends message
        /// </summary>
        /// <param name="msg"></param>
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
