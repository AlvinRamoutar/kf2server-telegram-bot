
namespace kf2server_tbot.Command {
    public class CMDRequest {

        public string Command { get; private set; }
        public string[] Args { get; private set; }
        public long ChatID { get; private set; }
        public long UserID { get; private set; }

        public CMDRequest(string commandText, string[] commandArgs, long chatID, long userID) {
            this.Command = commandText;
            this.Args = commandArgs;
            this.ChatID = chatID;
            this.UserID = userID;
        }

    }
}
