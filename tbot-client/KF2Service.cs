using LogEngine;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using tbot_client.KF2ServiceReference;
using Telegram.Bot.Args;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace tbot_client {

    /// <summary>
    /// Service consumer class, performs requests and returns data back to bot for chat relay
    /// </summary>
    class KF2Service : KF2ServiceClient {


        //private KF2ServiceClient _client { get; set; }


        /// <summary>
        /// Assigns Service credentials before starting
        /// </summary>
        public KF2Service() : base() {

            if(!string.IsNullOrWhiteSpace(Properties.Settings.Default.ServiceHostURI)) {
                this.Endpoint.Address = new EndpointAddress(new Uri(Properties.Settings.Default.ServiceHostURI),
                    this.Endpoint.Address.Identity,
                    this.Endpoint.Address.Headers);
            }

            this.ClientCredentials.UserName.UserName = Properties.Settings.Default.ServiceUsername;
            this.ClientCredentials.UserName.Password = Properties.Settings.Default.ServicePassword;

            try {
                this.Open();
            } catch(Exception e) {
                Logger.Log(LogEngine.Status.TELEGRAM_FAILURE, e.Message);
            }

        }


        /// <summary>
        /// Performs requests based on supplied command
        /// Acts on TEXT commands (e.g. '/cmd $argn' from chat)
        /// Awaits response, and formats message (to send back to telegram chat) based on prompts
        /// </summary>
        /// <param name="e">Message</param>
        /// <param name="command">Command in the form: '/cmd'</param>
        /// <param name="args">All arguments of command, including original command itself</param>
        /// <returns>Tuple result with message (sent back to chat), and service ResponseValue object</returns>
        public async Task<Tuple<string, ResponseValue>> CMD(MessageEventArgs e, string command, List<string> args) {

            ResponseValue tmpResponseValue = null;
            string tmpResponseMessage = string.Empty;

            /// Omitting the '/', and converting to completely lowercase, what is the command?
            switch(command.Substring(1).ToLower()) {

                #region Current Game

                case "changegametype":
                    tmpResponseValue = await this.ChangeGameTypeAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.ChangeGameType, args[1]);

                    break;

                case "changegametypeandmap":
                    tmpResponseValue = await this.ChangeGametypeAndMapAsync(args[1], args[2]);
                    tmpResponseMessage = string.Format(Prompts.ChangeGameTypeAndMap, args[1], args[2]);

                    break;

                case "changemap":
                    tmpResponseValue = await this.ChangeMapAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.ChangeMap, args[1]);

                    break;

                case "online":
                    tmpResponseValue = await this.OnlineAsync();
                    tmpResponseMessage = string.Format(Prompts.Online, tmpResponseValue.Data["online"]);

                    break;

                case "status":
                    tmpResponseValue = await this.StatusAsync();

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
                    tmpResponseValue = await this.GamePasswordOnAsync((args.Count == 1) ? null : args[1]);
                    tmpResponseMessage = string.Format(Prompts.GamePasswordOn, (args.Count == 1) ? "" : " to default in config");

                    break;

                case "gamepasswordoff":
                    tmpResponseValue = await this.GamePasswordOffAsync();
                    tmpResponseMessage = Prompts.GamePasswordOff;

                    break;

                #endregion

                #region Settings

                case "gamedifficulty":
                    tmpResponseValue = await this.GameDifficultyAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.GameDifficulty, args[1]);

                    break;

                case "gamelength":
                    tmpResponseValue = await this.GameLengthAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.GameLength, args[1]);

                    break;

                case "gamedifficultyandlength":
                    tmpResponseValue = await this.GameDifficultyAndLengthAsync(args[1], args[2]);
                    tmpResponseMessage = string.Format(Prompts.GameDifficultyAndLength, args[1], args[2]);

                    break;

                #endregion

                #region Miscellaneous

                case "adminsay":
                    tmpResponseValue = await this.AdminSayAsync(args[1]);
                    tmpResponseMessage = string.Format(Prompts.GameDifficultyAndLength, args[1], args[2]);

                    break;

                case "adduser":

                    tmpResponseValue = await this.AddUserAsync(e.Message.Entities[1].User.Id.ToString(), 
                        (e.Message.Entities.Length > 2) ? args[2].Split(',') : null);
                    tmpResponseMessage = string.Format(Prompts.AddUser, e.Message.Entities[1].User.Id.ToString());

                    break;

                case "removeuser":

                    tmpResponseValue = await this.RemoveUserAsync(e.Message.Entities[1].User.Id.ToString(),
                        (e.Message.Entities.Length > 2) ? args[2].Split(',') : null);
                    tmpResponseMessage = string.Format(Prompts.RemoveUser, e.Message.Entities[1].User.Id.ToString());

                    break;

                case "setup":
                    tmpResponseValue = await this.SetupAsync(e.Message.Contact.UserId.ToString(), e.Message.Chat.Id.ToString());

                    if (tmpResponseValue.IsSuccess)
                        tmpResponseMessage = string.Format(Prompts.ServiceSetupSuccess, Prompts.TBotServerName);
                    else
                        tmpResponseMessage = string.Format(Prompts.ServiceSetupFailure, Prompts.TBotServerName, tmpResponseValue.Message);
                    break;

                case "Test":
                    tmpResponseValue = await this.TestAsync();
                    tmpResponseMessage = tmpResponseValue.Message;

                    break;

                #endregion
            }

            return new Tuple<string, ResponseValue>(tmpResponseMessage, tmpResponseValue);
        }



    }

}
