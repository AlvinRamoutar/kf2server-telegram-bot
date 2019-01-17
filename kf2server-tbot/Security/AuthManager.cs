using LogEngine;
using System;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.Security {

    /// <summary>
    /// Simple custom Authorization and Authentication
    /// </summary>
    public class AuthManager {

        /// <summary>
        /// Users object containing data to authorize Telegram users
        /// </summary>
        public static Users Users { get; set; }


        /// <summary>
        /// Telegram Chat UUID, must be passed as parameter for all CMDs.
        /// </summary>
        private static string chatId;
        public static string ChatId {
            get {
                return chatId;
            }

            set {
                if(string.IsNullOrWhiteSpace(chatId))
                    chatId = value;
            }
        }




        /// <summary>
        /// Authorizer for service methods. Checks that a user can execute a method based on assigned roles in config.
        /// </summary>
        /// <param name="roleID">RoleID of service method</param>
        /// <param name="context">OperationContext of service, used to retrieve Telegram UUID and ChatId stored in header</param>
        /// <returns>True if user is authorized, else false.</returns>
        public static Tuple<bool, string> Authorize(string roleID, long chatID, long telegramID) {

            string msg = String.Empty;

            try {
                /// Retrieve user object from Account using hashed TelegramUUID
                Account userAcc = AuthManager.Users[Crypto.Hash(telegramID.ToString())];

                /// IF ChatId matches what is known by server
                if (chatID.Equals(ChatId)) {

                    /// IF Telegram user's account object contains role necessary for executing this operation, OR is admin
                    foreach (string userRole in userAcc.Roles.RoleID) {
                        if (userRole.ToLower().Equals(roleID.ToLower()) || userRole.ToLower().Equals("admin"))
                            return new Tuple<bool, string>(true, null);
                    }
                }

            } catch (Exception e) {

                msg = string.Format("User failed authorization for {0} (UUID: {1}). In addition, an error was thrown: {2}",
                    roleID, (string.IsNullOrEmpty(telegramID.ToString())) ? "Undefined" : Crypto.Hash(telegramID.ToString()), e.Message);

                Logger.Log(Status.SERVERADMIN_WARNING, msg);
                return new Tuple<bool, string>(false, msg);

            }

            msg = string.Format("User failed authorization for {0} (UUID: {1})",
                roleID, (string.IsNullOrEmpty(telegramID.ToString())) ? "Undefined" : Crypto.Hash(telegramID.ToString()));

            Logger.Log(Status.SERVERADMIN_WARNING, msg);
            return new Tuple<bool, string>(false, msg);
        }

    }

}