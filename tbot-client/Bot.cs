using System;
using System.Collections.Generic;
using tbot_client.KF2ServiceReference;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace tbot_client {


    /// <summary>
    /// Bot class, handles all bot-related operations, including sending/receiving from Telegram chat
    /// </summary>
    class Bot : TelegramBotClient {

        #region Properties and Fields
        public User Identity { get; private set; }

        public Chat Chat { get; private set; }

        private KF2Service _KF2Service { get; set; }

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

            _KF2Service = new KF2Service();
        }


        /// <summary>
        /// Performs assignment to Telegram chat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Setup(object sender, MessageEventArgs e) {

            await this.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: string.Format(Prompts.ServiceSetup, Prompts.TBotServerName)
            );

            /// Tries to assign ChatId to server
            /// Ideally, you would like to perform further authentication in server-side method 'Setup'
            Tuple<string, ResponseValue> result = await _KF2Service.CMD(e,
                "setup", new List<string>() { e.Message.Chat.Id.ToString() });

            /// If server isn't already binded to another chat
            if (result.Item2.IsSuccess) {

                Properties.Settings.Default.ChatId = e.Message.Chat.Id.ToString();
                Properties.Settings.Default.Save();
            }

            await this.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: result.Item1
            );

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

                Setup(sender, e);

            /// Otherwise, send message to KF2Service
            } else {

                Tuple<string, ResponseValue> result = await _KF2Service.CMD(e,
                    e.Message.Text.Split(' ')[0],
                    new List<string>(e.Message.Text.Split(' ')));

                
                await this.SendTextMessageAsync(
                    chatId: Chat.Id,
                    text: (string.IsNullOrWhiteSpace(result.Item1)) ? "This command does not exist." : result.Item1
                );
            }
        }

    }
}
