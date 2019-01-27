using kf2server_tbot.Security;
using kf2server_tbot.Utils;
using System.Collections.Generic;
using Telegram.Bot.Args;

namespace kf2server_tbot.Command {
    class Router {

        #region Properties and Fields

        private List<string> AnonymousCMDs = new List<string>() {
            "test"
        };

        #endregion
        private Commander Commander;

        public Router() {
            Commander = new Commander();
        }


        public string Request(MessageEventArgs e, string command, List<string> args) {

            return CMD(e, command, args);

        }
        
        
        /// <summary>
        /// Requests a command from Commander.
        /// Acts on TEXT commands (e.g. '/cmd $argn' from chat)
        /// Awaits response, and formats message (to send back to telegram chat) based on prompts
        /// </summary>
        /// <param name="e">Message</param>
        /// <param name="command">Command in the form: '/cmd'</param>
        /// <param name="args">All arguments of command, including original command itself</param>
        /// <returns>Tuple result with message (sent back to chat), and ResponseValue object</returns>
        private string CMD(MessageEventArgs e, string command, List<string> args) {

            ResponseValue tmpResponseValue = null;

            CMDRequest cmd = new CMDRequest(command.Substring(1).ToLower(), args.ToArray(), e.Message.Chat.Id, e.Message.From.Id);

            string tmpResponseMessage = Prompts.Invalid;

            switch (cmd.Command) {

                #region Current Game

                case "changegametype":
                    tmpResponseValue = Commander.ChangeGameType(cmd);
                    tmpResponseMessage = string.Format(Prompts.ChangeGameType, args[1]);

                    break;

                case "changegametypeandmap":
                    tmpResponseValue = Commander.ChangeGametypeAndMap(cmd);
                    tmpResponseMessage = string.Format(Prompts.ChangeGameTypeAndMap, args[1], args[2]);

                    break;

                case "changemap":
                    tmpResponseValue = Commander.ChangeMap(cmd);
                    tmpResponseMessage = string.Format(Prompts.ChangeMap, args[1]);

                    break;

                case "online":
                    tmpResponseValue = Commander.Online();
                    tmpResponseMessage = string.Format(Prompts.Online, tmpResponseValue.Data["online"]);

                    break;

                case "status":
                    tmpResponseValue = Commander.Status();

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

                case "gamepasswordon":
                    tmpResponseValue = Commander.GamePasswordOn(cmd);
                    tmpResponseMessage = string.Format(Prompts.GamePasswordOn, (args.Count == 1) ? "" : " to default in config");

                    break;

                case "gamepasswordoff":
                    tmpResponseValue = Commander.GamePasswordOff(cmd);
                    tmpResponseMessage = Prompts.GamePasswordOff;

                    break;

                #endregion

                #region Settings

                case "gamedifficulty":
                    tmpResponseValue = Commander.GameDifficulty(cmd);
                    tmpResponseMessage = string.Format(Prompts.GameDifficulty, args[1]);

                    break;

                case "gamelength":
                    tmpResponseValue = Commander.GameLength(cmd);
                    tmpResponseMessage = string.Format(Prompts.GameLength, args[1]);

                    break;

                case "gamedifficultyandlength":
                    tmpResponseValue = Commander.GameDifficultyAndLength(cmd);
                    tmpResponseMessage = string.Format(Prompts.GameDifficultyAndLength, args[1], args[2]);

                    break;

                #endregion

                #region Miscellaneous

                case "adminsay":
                    tmpResponseValue = Commander.AdminSay(cmd);
                    tmpResponseMessage = string.Format(Prompts.GameDifficultyAndLength, args[1], args[2]);

                    break;

                case "adduser":

                    tmpResponseValue = Commander.AddUser(cmd);
                    tmpResponseMessage = string.Format(Prompts.AddUser, e.Message.Entities[1].User.Id.ToString());
                    break;

                case "removeuser":

                    tmpResponseValue = Commander.RemoveUser(cmd);
                    tmpResponseMessage = string.Format(Prompts.RemoveUser, e.Message.Entities[1].User.Id.ToString());
                    break;

                case "setup":
                    tmpResponseMessage = string.Format(Prompts.Setup, cmd.ChatID);
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
