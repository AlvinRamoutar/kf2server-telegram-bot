using System;
using System.Collections.Generic;
using System.Linq;
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
            "Miscellaneous.AddUser",
            "Miscellaneous.RemoveUser",

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

        public static bool AddUser(Users user, string telegramUUID, string[] roles = null) {
            Account newUser = new Account();
            string hashedTelegramID = Crypto.Hash(telegramUUID);


            bool DoesUserExist = false;

            /// Check: Does user already exist?
            try {
                AuthManager.Users[hashedTelegramID].ToString();
                DoesUserExist = true;
            } catch (NullReferenceException) { }


            if (!DoesUserExist) { /// This user doesn't exist

                Role roleContainer = new Role();
                newUser.TelegramUUID = hashedTelegramID;
                List<string> tmpNewUserRoles = new List<string>();

                /// Create new Role object with all supplied roles
                foreach (string r in roles) {
                    if (Users.Roles.RoleID.Contains(r)) {
                        tmpNewUserRoles.Add(r);
                    }
                }

                /// Assign roles collection to newly created roles object
                roleContainer.RoleID = tmpNewUserRoles.ToArray<string>();
                newUser.Roles = roleContainer;

            } else { /// This user exists, performing role update

                newUser = AuthManager.Users[hashedTelegramID];
                List<string> tmpNewUserRoles = new List<string>(newUser.Roles.RoleID);

                /// Add role to users' role collection if they don't aready have it, and it exists (is a valid role)
                foreach (string r in roles) {
                    if (Users.Roles.RoleID.Contains(r) && !tmpNewUserRoles.Contains(r)) {
                        tmpNewUserRoles.Add(r);
                    }
                }

                /// Replaces users role collection with temp role collection (representing deltas)
                newUser.Roles.RoleID = tmpNewUserRoles.ToArray<string>();

            }

            AuthManager.Users.Accounts.Add(newUser);

            return DoesUserExist;
        }




        public static bool RemoveUser(Users user, string telegramUUID, string[] roles = null) {
            Account currUser = new Account();
            string hashedTelegramID = Crypto.Hash(telegramUUID);

            bool DoesUserExist = false;

            /// Check: Does user already exist?
            try {
                AuthManager.Users[hashedTelegramID].ToString();
                DoesUserExist = true;
            } catch (NullReferenceException) { }


            if (DoesUserExist) { /// This user exists, performing role update / user removal

                /// Check: Are roles specified to remove? If none are, then assuming USER will be deleted completely
                if(roles.Length == 0) {
                    AuthManager.Users.Accounts.Remove(AuthManager.Users[hashedTelegramID]);

                    return true;
                }

                currUser = AuthManager.Users[hashedTelegramID];
                List<string> tmpNewUserRoles = new List<string>(currUser.Roles.RoleID);

                /// Remove roles from users' role collection if they aready have it, and it exists (is a valid role)
                foreach (string r in roles) {
                    if (Users.Roles.RoleID.Contains(r) && tmpNewUserRoles.Contains(r)) {
                        tmpNewUserRoles.Remove(r);
                    }
                }

                /// Replaces users role collection with temp role collection (representing deltas)
                currUser.Roles.RoleID = tmpNewUserRoles.ToArray<string>();

            }

            AuthManager.Users[hashedTelegramID].Roles = currUser.Roles;

            return DoesUserExist;
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
