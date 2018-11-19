using System;
using System.Threading.Tasks;
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

            this.StartReceiving();

            this.OnMessage += this.OnMessageHandler;
        }


        public async void Setup(string telegramUUID, Chat chat) {



        }


        /// <summary>
        /// Routes to specific handler based on kind of message (entity or document)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageHandler(object sender, MessageEventArgs e) {

            if (e.Message.Text != null) {

                OnMessageEntitiesHandler(sender, e);

            } else if(e.Message.Document != null) {

                OnMessageDocumentHandler(sender, e);

            }

        }


        private async void OnMessageEntitiesHandler(object sender, MessageEventArgs e) {

            if(e.Message.Entities[0].ToString().Equals("setup")) {

                this.OnMessage -= this.OnMessageHandler;
                this.OnMessage += this.OnMessageSetupHandler;

                await this.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: string.Format(Prompts.TokenRequest, Prompts.TBotServerName)
                );

                _SetupTelegramUUID = e.Message.Contact.UserId.ToString();

                return;
            }



            await this.SendTextMessageAsync(
                chatId: Chat.Id,
                text: await _KF2Service.CMD(e.Message.Contact.UserId.ToString(), 
                    e.Message.Entities[0].ToString(), 
                    e.Message.EntityValues)
            );

        }


        /// <summary>
        /// Mainly for handling token file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnMessageSetupHandler(object sender, MessageEventArgs e) {

            if(e.Message.Contact.UserId.ToString().Equals(_SetupTelegramUUID)) {

                if(e.Message.Document != null) {

                    File token = await this.GetFileAsync(e.Message.Document.FileId);

                    _KF2Service.ValidateToken(token);


                }

            }

        }

    }
}
