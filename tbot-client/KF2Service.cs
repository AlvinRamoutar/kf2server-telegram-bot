using LogEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using tbot_client.KF2ServiceReference;
using Telegram.Bot.Args;

namespace tbot_client {

    class KF2Service : KF2ServiceClient {


        private KF2ServiceClient _client { get; set; }


        public KF2Service() : base() {

            _client = new KF2ServiceClient();

            _client.ClientCredentials.UserName.UserName = Properties.Settings.Default.ServiceUsername;
            _client.ClientCredentials.UserName.Password = Properties.Settings.Default.ServicePassword;

            try {
                _client.Open();
            } catch(Exception e) {
                Logger.Log(LogEngine.Status.SERVICE_FAILURE, e.Message);
            }

        }


        public async Task<Tuple<string, ResponseValue>> CMD(MessageEventArgs e, string command, List<string> args) {

            ResponseValue tmpResponseValue = null;
            string tmpResponseMessage = null;

            switch(command.Substring(1).ToLower()) {

                case "setup":
                    tmpResponseValue = await _client.SetupAsync(e.Message.Chat.Id.ToString());
                    if (tmpResponseValue.IsSuccess) {
                        tmpResponseMessage = string.Format(Prompts.ServiceSetupSuccess, Prompts.TBotServerName);
                    }
                    else {
                        tmpResponseMessage = string.Format(Prompts.ServiceSetupFailure, Prompts.TBotServerName, tmpResponseValue.Message);
                    }
                    break;

                case "Test":
                    tmpResponseValue = await _client.TestAsync();
                    break;
            }

            return new Tuple<string, ResponseValue>(tmpResponseMessage, tmpResponseValue);
        }



    }

}
