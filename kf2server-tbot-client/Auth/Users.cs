using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace kf2server_tbot_client.Auth {

    [Serializable, XmlRoot("Users")]
    public class Users {

        [XmlElement("Accounts")]
        public List<Account> Accounts { get; set; }

        [XmlElement("RoleIDList")]
        public Role Roles = new Role(new string[] {
            "Miscellaneous.Test",
            "Miscellaneous.AdminSay"
        });

        public Users() {
            this.Accounts = new List<Account>();
        }


        public Account this[string telegramID] {
            get {
                return Accounts.Find(acc => acc.Username == telegramID);
            }
        }

    }

    [Serializable, XmlRoot("Account")]
    public class Account {

        [XmlElement("TelegramID")]
        public string Username { get; set; }

        [XmlElement("SteamUUID")]
        public string Password { get; set; }

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
