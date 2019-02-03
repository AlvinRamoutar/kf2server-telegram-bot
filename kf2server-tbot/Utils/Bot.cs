﻿using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using kf2server_tbot.Command;
using kf2server_tbot.Security;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.Utils {


    public enum SetupStage {
        SupplyChatId,
        HandshakeMessage,
        PostSetup
    }


    /// <summary>
    /// Bot class, handles all bot-related operations, including sending/receiving from Telegram chat
    /// </summary>
    class Bot : TelegramBotClient {

        #region Properties and Fields
        public User Identity { get; private set; }

        public Chat Chat { get; private set; }

        private User SetupTelegramUser = null;

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
        /// Routes to specific handler based on kind of message (entity or document)
        /// </summary>
        /// <param name="sender">Telegram</param>
        /// <param name="e">Message</param>
        private void OnMessageHandler(object sender, MessageEventArgs e) {

            if (e.Message.Text != null) {

                OnMessageEntitiesHandler(sender, e);

            } 

        }
        


        public async void Setup(SetupStage stage, MessageEventArgs e, List<string> args) {

            switch(stage) {

                case SetupStage.SupplyChatId:
                    await this.SendTextMessageAsync(
                        chatId: e.Message.Chat.Id,
                        text: string.Format(Prompts.Setup, e.Message.Chat.Id)
                    );

                    // Save initializer user's ID
                    SetupTelegramUser = e.Message.From;
                    LogEngine.Logger.Log(LogEngine.Status.TELEGRAM_INFO, 
                        string.Format("Telegram UUID {0} initialized setup!", SetupTelegramUser));
                    break;

                case SetupStage.HandshakeMessage:

                    try {
                        await this.SendTextMessageAsync(
                            chatId: args[0],
                            text: string.Format("Successfully bound to bot.", Prompts.TBotServerName)
                        );
                        // Save chat object
                        Chat = this.GetChatAsync(args[0]).Result;

                    } catch(Telegram.Bot.Exceptions.ChatNotFoundException) {
                        LogEngine.Logger.Log(LogEngine.Status.TELEGRAM_FAILURE, "Bot is not currently a part of '" + args[0] + "'");
                    }
                    break;
                    
                case SetupStage.PostSetup:

                    Properties.Settings.Default.ChatId = Chat.Id.ToString();
                    Properties.Settings.Default.Save();

                    AuthManager.ChatId = Chat.Id.ToString();

                    Router.Commander.AddUser(new CMDRequest("/adduser", 
                        new string[] { "/adduser", SetupTelegramUser.ToString(), "admin" },
                        Chat.Id, SetupTelegramUser, "setup"), SetupTelegramUser);
                    break;
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

            
            if (Chat == null || e.Message.Text.ToLower().Contains("/setup")) { /// If setup hasn't been performed yet

                Setup(SetupStage.SupplyChatId, e, null);
                
            } else {

                string cmd = (e.Message.Text.Contains(" ")) ? e.Message.Text.Split(' ')[0] : e.Message.Text;
                string[] args = (e.Message.Text.Contains(" ")) ? e.Message.Text.Split(' ') : new string[] { e.Message.Text };

                string result = Router.Request(e, cmd, new List<string>(args));

                await this.SendTextMessageAsync(Chat.Id, result);
            }
        }

    }
}
