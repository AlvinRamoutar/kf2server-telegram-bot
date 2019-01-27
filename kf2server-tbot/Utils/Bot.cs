using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using kf2server_tbot.Utils;
using kf2server_tbot.Command;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.Utils {


    /// <summary>
    /// Bot class, handles all bot-related operations, including sending/receiving from Telegram chat
    /// </summary>
    class Bot : TelegramBotClient {

        #region Properties and Fields
        public User Identity { get; private set; }

        public Chat Chat { get; private set; }

        private Router Router { get; set; }

        #endregion


        /// <summary>
        /// Constructs new bot, retrieve bot identity, assign ChatId (if one is stored), 
        ///  starts receiving, attaches message handlers, and kickstarts service consumer
        /// </summary>
        /// <param name="token">Telegram Bot Token</param>
        public Bot(string token) : base(token) {

            Identity = this.GetMeAsync().Result;

            Chat = (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ChatId)) ?
                this.GetChatAsync(Properties.Settings.Default.ChatId).Result : null;

            this.StartReceiving();

            this.OnMessage += this.OnMessageHandler;

            Router = new Router();
        }


        /// <summary>
        /// Performs assignment to Telegram chat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void ReturnedSetupMessage(string chatID) {

            await this.SendTextMessageAsync(
                chatId: long.Parse(chatID),
                text: string.Format("Successfully bound to bot.", Prompts.TBotServerName)
            );

            // Save chat object
            Chat = this.GetChatAsync(chatID).Result;

        }


        /// <summary>
        /// Routes to specific handler based on kind of message (entity or document)
        /// </summary>
        /// <param name="sender">Telegram</param>
        /// <param name="e">Message</param>
        private void OnMessageHandler(object sender, MessageEventArgs e) {

            if (e.Message.Text != null) {

                OnMessageEntitiesHandler(sender, e);

            } 

        }


        /// <summary>
        /// Directs message to appropriate response method.
        /// <para>In the case where setup hasn't been ran (no ChatId assigned), then that is done.</para>
        /// <para>Else, command sent to KF2Service, where it tries to execute cmd based on existing service methods</para>
        /// </summary>
        /// <param name="sender">Telegram</param>
        /// <param name="e">Message</param>
        private async void OnMessageEntitiesHandler(object sender, MessageEventArgs e) {

            /// If setup hasn't been performed yet
            if(Chat == null) {

                await this.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: string.Format(Prompts.Setup, e.Message.Chat.Id)
                );
    
            /// Otherwise, pass message to Router
            } else {

                string cmd = (e.Message.Text.Contains(" ")) ? e.Message.Text.Split(' ')[0] : e.Message.Text;
                string[] args = (e.Message.Text.Contains(" ")) ? e.Message.Text.Split(' ') : new string[] {  e.Message.Text };

                string result = Router.Request(e, cmd, new List<string>(args));

                await this.SendTextMessageAsync(Chat.Id, result);
            }
        }

    }
}
