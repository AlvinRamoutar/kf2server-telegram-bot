
using Telegram.Bot.Types;

namespace kf2server_tbot.Command {
    public class CMDRequest {

        public string Command { get; private set; }
        public string[] Args { get; private set; }
        public long ChatID { get; private set; }
        public User User { get; set; }
        public object Data { get; set; }

        public CMDRequest(string commandText, string[] commandArgs, long chatID,
            User user = null, object data = null) {

            this.Command = commandText;
            this.Args = commandArgs;
            this.ChatID = chatID;
            this.User = user;
            this.Data = data;
        }
    }
}
