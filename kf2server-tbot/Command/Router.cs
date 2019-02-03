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
        public Commander Commander;

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
