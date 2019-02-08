using kf2server_tbot.Utils;
using System.Collections.Generic;
using Telegram.Bot.Args;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.Command {
    class Router {

        #region Properties and Fields

        private List<string> AnonymousCMDs = new List<string>() {
            "test"
        };

        public Commander Commander;

        #endregion

        public Router() {
            Commander = new Commander();
        }


        /// <summary>
        /// Pre-route method for logic to occur before routing command to appropriate Commander method.
        /// </summary>
        /// <param name="e">Telegram Message object</param>
        /// <param name="command">Command verb following '/' ending at space.</param>
        /// <param name="args">Split (space as delimiter) of command message. Includes command.</param>
        /// <returns></returns>
        public string Request(MessageEventArgs e, string command, List<string> args) {

            return CMD(e, command, args);

        }


        /// <summary>
        /// Performs call to appropriate method in Commander.
        /// Acts on commands (e.g. '/cmd $argn' from chat)
        /// Returns a 'will-do' prompt which will be pushed to chat
        /// </summary>
        /// <param name="e">Telegram Message object</param>
        /// <param name="command">Command verb following '/' ending at space.</param>
        /// <param name="args">Split (space as delimiter) of command message. Includes command.</param>
        /// <returns>Tuple result with message (sent back to chat), and ResponseValue object</returns>
        private string CMD(MessageEventArgs e, string command, List<string> args) {

            ResponseValue tmpResponseValue = null;

            /// Build CMDRequest object to hold message/command data for Commander methods
            CMDRequest cmd = new CMDRequest(command.Substring(1).ToLower(), args.ToArray(), 
                e.Message.Chat.Id , e.Message.From, null);

            string tmpResponseMessage = Prompts.Invalid;

            switch (cmd.Command) {

                #region Current Game

                case "gametype":
                    tmpResponseValue = Commander.ChangeGameType(cmd);
                    tmpResponseMessage = string.Format(Prompts.ChangeGameType, args[1]);

                    break;

                case "gametypeandmap":
                    tmpResponseValue = Commander.ChangeGametypeAndMap(cmd);
                    tmpResponseMessage = string.Format(Prompts.ChangeGameTypeAndMap, args[1], args[2]);

                    break;

                case "map":
                    tmpResponseValue = Commander.ChangeMap(cmd);
                    tmpResponseMessage = string.Format(Prompts.ChangeMap, args[1]);

                    break;

                case "online":
                    tmpResponseValue = Commander.Online();
                    tmpResponseMessage = string.Format(Prompts.Online, tmpResponseValue.Data["online"]);

                    break;

                case "status":
                    tmpResponseValue = Commander.Status();
                    tmpResponseMessage = string.Empty;

                    if (tmpResponseValue.IsSuccess) {
                        foreach (KeyValuePair<string, string> kvp in tmpResponseValue.Data) {
                            tmpResponseMessage += kvp.Key + ": " + kvp.Value + "\n";
                        }
                    }
                    else {
                        tmpResponseMessage = string.Format(Prompts.StatusFailure, Prompts.TBotServerName);
                    }
                    break;

                #endregion

                #region Access Policy

                case "pwdon":
                    tmpResponseValue = Commander.GamePasswordOn(cmd);
                    tmpResponseMessage = string.Format(Prompts.GamePasswordOn, (args.Count == 1) ? " to default in config" : "");

                    break;

                case "pwdoff":
                    tmpResponseValue = Commander.GamePasswordOff(cmd);
                    tmpResponseMessage = Prompts.GamePasswordOff;

                    break;

                #endregion

                #region Settings

                case "difficulty":
                    tmpResponseValue = Commander.GameDifficulty(cmd);
                    tmpResponseMessage = string.Format(Prompts.GameDifficulty, args[1]);

                    break;

                case "length":
                    tmpResponseValue = Commander.GameLength(cmd);
                    tmpResponseMessage = string.Format(Prompts.GameLength, args[1]);

                    break;

                case "difficultyandlength":
                    tmpResponseValue = Commander.GameDifficultyAndLength(cmd);
                    tmpResponseMessage = string.Format(Prompts.GameDifficultyAndLength, args[1], args[2]);

                    break;

                #endregion

                #region Miscellaneous

                case "adminsay":
                    tmpResponseValue = Commander.AdminSay(cmd);

                    string msg = "";
                    for (int i = 1; i < args.Count; i++)
                        msg += " " + args[i];
                    tmpResponseMessage = string.Format(Prompts.AdminSay, msg);

                    break;

                case "adduser":

                    if(e.Message.Entities[1].Type == Telegram.Bot.Types.Enums.MessageEntityType.TextMention) {                           
                        tmpResponseValue = Commander.AddUser(cmd, e.Message.Entities[1].User);
                        tmpResponseMessage = string.Format(Prompts.AddUser, e.Message.Entities[1].User.ToString());
                    }
                    break;

                case "removeuser":

                    if (e.Message.Entities[1].Type == Telegram.Bot.Types.Enums.MessageEntityType.TextMention) {
                        tmpResponseValue = Commander.RemoveUser(cmd, e.Message.Entities[1].User);
                        tmpResponseMessage = string.Format(Prompts.RemoveUser, e.Message.Entities[1].User.ToString());
                    }
                    break;

                case "Test":
                    tmpResponseValue = Commander.Test();
                    tmpResponseMessage = tmpResponseValue.Message;

                    break;

                    #endregion
            }

            return tmpResponseMessage;
        }


    }
}
