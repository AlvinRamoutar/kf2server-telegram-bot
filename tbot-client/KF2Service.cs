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
            string tmpResponseMessage = string.Empty;

            switch(command.Substring(1).ToLower()) {

                #region Current Game

                case "changegametype":
                    tmpResponseValue = await _client.ChangeGameTypeAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.ChangeGameType, args[1]);

                    break;

                case "changegametypeandmap":
                    tmpResponseValue = await _client.ChangeGametypeAndMapAsync(args[1], args[2]);
                    tmpResponseMessage = string.Format(Prompts.ChangeGameTypeAndMap, args[1], args[2]);

                    break;

                case "changemap":
                    tmpResponseValue = await _client.ChangeMapAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.ChangeMap, args[1]);

                    break;

                case "online":
                    tmpResponseValue = await _client.OnlineAsync();
                    tmpResponseMessage = string.Format(Prompts.Online, tmpResponseValue.Data["online"]);

                    break;

                case "status":
                    tmpResponseValue = await _client.StatusAsync();

                    if (tmpResponseValue.IsSuccess) {
                        foreach (KeyValuePair<string, string> kvp in tmpResponseValue.Data) {
                            tmpResponseMessage += kvp.Key + ": " + kvp.Value + "\n";
                        }
                    } else {
                        tmpResponseMessage = string.Format(Prompts.StatusFailure, Prompts.TBotServerName);
                    }
                    break;

                #endregion

                #region Access Policy

                case "gamepasswordon":
                    tmpResponseValue = await _client.GamePasswordOnAsync((args.Count == 1) ? null : args[1]);
                    tmpResponseMessage = string.Format(Prompts.GamePasswordOn, (args.Count == 1) ? "" : " to default in config");

                    break;

                case "gamepasswordoff":
                    tmpResponseValue = await _client.GamePasswordOffAsync();
                    tmpResponseMessage = Prompts.GamePasswordOff;

                    break;

                #endregion

                #region Settings

                case "gamedifficulty":
                    tmpResponseValue = await _client.GameDifficultyAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.GameDifficulty, args[1]);

                    break;

                case "gamelength":
                    tmpResponseValue = await _client.GameLengthAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.GameLength, args[1]);

                    break;

                case "gamedifficultyandlength":
                    tmpResponseValue = await _client.GameDifficultyAndLengthAsync(args[1], args[2]);
                    tmpResponseMessage = string.Format(Prompts.GameDifficultyAndLength, args[1], args[2]);

                    break;

                #endregion

                #region Miscellaneous

                case "adminsay":
                    tmpResponseValue = await _client.AdminSayAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.GameDifficultyAndLength, args[1], args[2]);

                    break;

                /// WIP
                case "adduser":

                    tmpResponseValue = await _client.AddUserAsync(args[1], args[2].Split(','));
                    tmpResponseMessage = string.Format(Prompts.GameDifficultyAndLength, args[1], args[2]);

                    break;

                /// WIP
                case "removeuser":

                    tmpResponseValue = await _client.AddUserAsync(args[1], args[2].Split(','));
                    tmpResponseMessage = string.Format(Prompts.GameDifficultyAndLength, args[1], args[2]);

                    break;

                case "setup":
                    tmpResponseValue = await _client.SetupAsync(e.Message.Chat.Id.ToString());

                    if (tmpResponseValue.IsSuccess)
                        tmpResponseMessage = string.Format(Prompts.ServiceSetupSuccess, Prompts.TBotServerName);
                    else
                        tmpResponseMessage = string.Format(Prompts.ServiceSetupFailure, Prompts.TBotServerName, tmpResponseValue.Message);
                    break;

                case "Test":
                    tmpResponseValue = await _client.TestAsync();
                    tmpResponseMessage = tmpResponseValue.Message;

                    break;

                #endregion
            }

            return new Tuple<string, ResponseValue>(tmpResponseMessage, tmpResponseValue);
        }



    }

}
