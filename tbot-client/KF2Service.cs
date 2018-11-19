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


        public async Task<string> CMD(string telegramUUID, string command, IEnumerable<string> args) {

            ResponseValue tmpResponseValue = null;
            string tmpResponseMessage = null;

            switch(command) {

                case "Test":
                    tmpResponseValue = await _client.TestAsync();
                    tmpResponseMessage = tmpResponseValue.Message;
                    break;

            }

            return tmpResponseMessage;

        }


        public async Task<bool> ValidateToken(File tokenFile) {

        }


    }

}
