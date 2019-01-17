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
            "miscellaneous.setup",

            "settings.gamedifficulty",
            "settings.gamelength",
            "settings.gamedifficultyandlength",
            
            "admin"
        });

        public Users() {
            this.Accounts = new List<Account>();
        }


        public Account this[string telegramID] {
            get {
                return Accounts.Find(acc => acc.TelegramUUID == telegramID);
            }
        }


        /// <summary>
        /// Adds a new user to the active Users object, or updates the roles assigned to an existing user (appends)
        /// </summary>
        /// <param name="user">Active users object</param>
        /// <param name="telegramUUID">Telegram UUID</param>
        /// <param name="roles">Roles to append to user</param>
        /// <returns></returns>
        public static bool AddUser(Users user, long telegramUUID, string[] roles = null) {

            Account newUser = new Account();
            string hashedTelegramID = Crypto.Hash(telegramUUID.ToString());

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
                string tmpr = string.Empty;
                foreach (string r in roles) {
                    tmpr = r.ToLower().Trim();

                    /// If supplied role is in the format $PAGECATEGORY.$ROLE
                    if (Users.Roles.RoleID.Contains(tmpr)) {
                        tmpNewUserRoles.Add(tmpr);
                    
                    /// If supplied role is in the format $ROLE (shorthand)
                    } else {

                        foreach(string shorthandRole in Users.Roles.RoleID) {
                            if(shorthandRole.Substring(shorthandRole.IndexOf('.') + 1).ToLower().Equals(tmpr))
                                tmpNewUserRoles.Add(shorthandRole);
                        }
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



        /// <summary>
        /// Removes a user from the active Users object, or updates the roles assigned to an existing user (pops)
        /// If no roles are supplied, then the user is deleted.
        /// </summary>
        /// <param name="user">Active users object</param>
        /// <param name="telegramUUID">Telegram UUID</param>
        /// <param name="roles">Roles to pop from user. If all roles from user is popped, user is deleted.</param>
        /// <returns></returns>
        public static bool RemoveUser(Users user, long telegramUUID, string[] roles = null) {
            Account currUser = new Account();
            string hashedTelegramID = Crypto.Hash(telegramUUID.ToString());

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
