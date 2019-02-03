using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.Security {

    /// <summary>
    /// Serializable class containing user information, as well as role assignments
    /// </summary>
    [Serializable, XmlRoot("Users")]
    public class Users {

        [XmlElement("Accounts")]
        public List<Account> Accounts { get; set; }

        [XmlElement("RoleIDList")]
        public static Role Roles = new Role(new string[] {
            "accesspolicy.gamepasswordon",
            "accesspolicy.gamepasswordoff",

            "currentgame.changegametype",
            "currentgame.changegametypeandmap",
            "currentgame.changemap",
            "currentgame.online",
            "currentgame.kick",
            "currentgame.status",

            "miscellaneous.adminsay",
            "miscellaneous.pause",
            "miscellaneous.test",
            "miscellaneous.adduser",
            "miscellaneous.removeuser",

            "settings.gamedifficulty",
            "settings.gamelength",
            "settings.gamedifficultyandlength",
            
            "admin"
        });

        public Users() {
            this.Accounts = new List<Account>();
        }


        public Account this[long telegramUUID] {
            get {
                return Accounts.Find(acc => acc.TelegramUUID == telegramUUID);
            }
        }

        public Account this[string telegramUsername] {
            get {
                return Accounts.Find(acc => acc.TelegramUsername == telegramUsername);
            }
        }


        /// <summary>
        /// Adds a user to the active Users object.
        /// Essentially how bot is made aware of users.
        /// </summary>
        /// <param name="telegramUUID">Telegram UUID</param>
        /// <param name="telegramUsername">Telegram User's Username (or if no username, then First Name)</param>
        public static void OptIn(long telegramUUID, string telegramUsername) {

            Account newUser = new Account();

            /// Check: Does user already exist?
            newUser = AuthManager.Users[telegramUUID];
            if (newUser != null)
                return;


            Role roleContainer = new Role();
            roleContainer.RoleID = new string[] { "miscellaneous.optin" };
            
            newUser.TelegramUsername = telegramUsername;
            newUser.TelegramUUID = telegramUUID;
            newUser.Roles = roleContainer;


            AuthManager.Users.Accounts.Add(newUser);
        }


        /// <summary>
        /// Adds a new user to the active Users object, or updates the roles assigned to an existing user (appends)
        /// </summary>
        /// <param name="telegramUUID">Telegram UUID</param>
        /// <param name="telegramUsername">Telegram Username, or if none, then First Name</param>
        /// <param name="roles">Roles to append to user</param>
        /// <returns></returns>
        public static bool AddUser(Telegram.Bot.Types.User mentionedUser, string[] roles = null) {

            Account newUser;

            bool isNewUser = false;

            /// Check: Does user already exist? Else, create new account object.
            newUser = AuthManager.Users[
                (string.IsNullOrEmpty(mentionedUser.Username)) ? mentionedUser.FirstName : mentionedUser.Username];
            if (newUser == null) {
                isNewUser = true;
                newUser = new Account();
                newUser.TelegramUUID = mentionedUser.Id;
                newUser.TelegramUsername =
                    (string.IsNullOrEmpty(mentionedUser.Username)) ? mentionedUser.FirstName : mentionedUser.Username;
            }

            Role roleContainer = new Role();
            List<string> tmpNewUserRoles = new List<string>();
            if (!isNewUser)
                tmpNewUserRoles = new List<string>(newUser.Roles.RoleID);

            /// Create new Role object with all supplied roles
            string tmpr = string.Empty;
            foreach (string r in roles) {
                tmpr = r.ToLower().Trim();

                /// If supplied role is in the format $PAGECATEGORY.$ROLE
                if (Users.Roles.RoleID.Contains(tmpr) && !tmpNewUserRoles.Exists(role => role.Equals(tmpr))) {
                    tmpNewUserRoles.Add(tmpr);
                    
                /// If supplied role is in the format $ROLE (shorthand)
                } else {

                    foreach(string shorthandRole in Users.Roles.RoleID) {
                        if((shorthandRole.Substring(shorthandRole.IndexOf('.') + 1).ToLower().Equals(tmpr)) &&
                            !tmpNewUserRoles.Exists(role => role.Equals(tmpr)))
                            tmpNewUserRoles.Add(shorthandRole);
                    }
                }
            }

            /// Assign roles collection to newly created roles object
            roleContainer.RoleID = tmpNewUserRoles.ToArray<string>();
            newUser.Roles = roleContainer;

            AuthManager.Users.Accounts.Remove(newUser);
            AuthManager.Users.Accounts.Add(newUser);

            return true;
        }



        /// <summary>
        /// Removes a user from the active Users object, or updates the roles assigned to an existing user (pops)
        /// If no roles are supplied, then the user is deleted.
        /// </summary>
        /// <param name="user">Active users object</param>
        /// <param name="telegramUUID">Telegram UUID</param>
        /// <param name="roles">Roles to pop from user. If all roles from user is popped, user is deleted.</param>
        /// <returns></returns>
        public static bool RemoveUser(Telegram.Bot.Types.User mentionedUser, string[] roles = null) {

            Account currUser;

            bool doesUserExist = true;

            /// Check: Does user already exist?
            currUser = AuthManager.Users[mentionedUser.Id];
            if (currUser == null)
                doesUserExist = false;


            if (doesUserExist) { /// This user exists, performing role update / user removal

                /// Check: Are roles specified to remove? If none are, then assuming USER will be deleted completely
                if(roles.Length == 0) {
                    AuthManager.Users.Accounts.Remove(AuthManager.Users[mentionedUser.Id]);

                    return true;
                }

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

            AuthManager.Users.Accounts.Remove(currUser);
            AuthManager.Users.Accounts.Add(currUser);

            return doesUserExist;
        }

    }

    [Serializable, XmlRoot("Account")]
    public class Account {

        [XmlElement("TelegramUUID")]
        public long TelegramUUID { get; set; }

        [XmlElement("TelegramUsername")]
        public string TelegramUsername { get; set; }

        [XmlElement("Roles")]
        public Role Roles { get; set; }

        public Account() {
            Roles = new Role();
        }

        public override bool Equals(object obj) {

            Account compAcc = obj as Account;
            return (compAcc.TelegramUUID == TelegramUUID);
        }

        public override int GetHashCode() {

            var hashCode = 1170669641;
            hashCode = hashCode * -1521134295 + TelegramUUID.GetHashCode();
            return hashCode;
        }
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
