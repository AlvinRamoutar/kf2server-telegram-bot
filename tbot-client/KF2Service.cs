using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using tbot_client.KF2ServiceReference;

namespace tbot_client {

    class KF2Service : KF2ServiceClient {


        private KF2ServiceClient _client { get; set; }


        public KF2Service() : base() {

            _client = new KF2ServiceClient();

            _client.ClientCredentials.UserName.UserName = Properties.Settings.Default.ServiceUsername;
            _client.ClientCredentials.UserName.Password = Properties.Settings.Default.ServicePassword;

            _client.Open();

        }


        public async Task<Tuple<string, ResponseValue>> CMD(string telegramUUID, string command, List<string> args) {

            ResponseValue tmpResponseValue = null;
            string tmpResponseMessage = null;

            switch(command) {

                case "setup":
                    tmpResponseValue = await _client.SetupAsync(args[0]);
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
