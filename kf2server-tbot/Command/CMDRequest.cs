using Telegram.Bot.Types;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.Command {

    /// <summary>
    /// Holds vital parameters which construct a command call
    /// </summary>
    public class CMDRequest {

        #region Properties and Fields
        public string Command { get; private set; }
        public string[] Args { get; private set; }
        public long ChatID { get; private set; }
        public User User { get; set; }
        public object Data { get; set; }
        #endregion

        #region Constructors
        public CMDRequest(string commandText, string[] commandArgs, long chatID,
            User user = null, object data = null) {

            this.Command = commandText;
            this.Args = commandArgs;
            this.ChatID = chatID;
            this.User = user;
            this.Data = data;
        }
        #endregion

    }
}
