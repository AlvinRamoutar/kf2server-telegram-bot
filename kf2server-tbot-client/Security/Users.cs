using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace kf2server_tbot_client.Security {

    [Serializable, XmlRoot("Users")]
    public class Users {

        [XmlElement("Accounts")]
        public List<Account> Accounts { get; set; }

        [XmlElement("RoleIDList")]
        public static Role Roles = new Role(new string[] {
            "AccessPolicy.GamePasswordOn",
            "AccessPolicy.GamePasswordOff",

            "CurrentGame.ChangeGameType",
            "CurrentGame.ChangeGametypeAndMap",
            "CurrentGame.ChangeMap",
            "CurrentGame.Players",
            "CurrentGame.Players_KickPlayer",
            "CurrentGame.Status",

            "Miscellaneous.AdminSay",
            "Miscellaneous.Pause",
            "Miscellaneous.Test",

            "Settings.General_Game_GameDifficulty",
            "Settings.General_Game_GameLength",
            "Settings.General_Game_GameDifficultyAndLength"
        });

        public Users() {
            this.Accounts = new List<Account>();
        }


        public Account this[string telegramID] {
            get {
                return Accounts.Find(acc => acc.TelegramUUID == telegramID);
            }
        }

    }

    [Serializable, XmlRoot("Account")]
    public class Account {

        [XmlElement("TelegramID")]
        public string TelegramUUID { get; set; }

        [XmlElement("SteamUUID")]
        public string SteamUUID { get; set; }

        [XmlElement("Roles")]
        public Role Roles { get; set; }

    }


    [Serializable, XmlRoot("Role")]
    public class Role {

        [XmlElement("RoleID")]
        public string[] RoleID { get; set; }

        public Role() { }

        public Role(string[] roles) {
            RoleID = roles;
        }
    }

}
