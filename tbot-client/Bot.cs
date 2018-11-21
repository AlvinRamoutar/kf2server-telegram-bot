using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tbot_client.KF2ServiceReference;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace tbot_client {

    class Bot : TelegramBotClient {

        #region Properties and Fields
        public User Identity { get; private set; }

        public Chat Chat { get; private set; }

        private KF2Service _KF2Service { get; set; }

        private string _SetupTelegramUUID { get; set; }
        #endregion


        public Bot(string token) : base(token) {

            Identity = this.GetMeAsync().Result;

            Chat = (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ChatId)) ?
                this.GetChatAsync(Properties.Settings.Default.ChatId).Result : null;

            this.StartReceiving();

            this.OnMessage += this.OnMessageHandler;

            _KF2Service = new KF2Service();

            //this.Welcome();
        }


        public async void Welcome() {

            await this.SendTextMessageAsync(
                chatId: Chat.Id,
                text: "Welcome, to the Jungle."
            );

        }



        public async void Setup(object sender, MessageEventArgs e) {

            await this.SendTextMessageAsync(
                chatId: e.Message.Chat.Id,
                text: string.Format(Prompts.ServiceSetup, Prompts.TBotServerName)
            );


            Tuple<string, ResponseValue> result = await _KF2Service.CMD(e,
                "setup", new List<string>() { e.Message.Chat.Id.ToString() });


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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageHandler(object sender, MessageEventArgs e) {

            if (e.Message.Text != null) {

                OnMessageEntitiesHandler(sender, e);

            } 

        }


        private async void OnMessageEntitiesHandler(object sender, MessageEventArgs e) {

            if(Chat == null) {

                Setup(sender, e);
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
