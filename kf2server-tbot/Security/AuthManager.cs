using LogEngine;
using System;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.Security {

    /// <summary>
    /// Authorization tool for ensuring Users can perform particular Commander methods
    /// </summary>
    public class AuthManager {

        #region Properties and Fields

        public static Users Users { get; set; }

        public static string ChatId { get; set; }

        #endregion

        /// <summary>
        /// Authorization handler for Commander methods.
        /// <para>>Determine if a particular user (based on supplied telegramUUID) has a RoleID which matches that
        ///  of the caller (Commander method)</para
        /// </summary>
        /// <param name="roleID">RoleID of service method</param>
        /// <param name="context">OperationContext of service, used to retrieve Telegram UUID and ChatId stored in header</param>
        /// <returns>True if user is authorized, else false.</returns>
        public static Tuple<bool, string> Authorize(string roleID, long chatID, long telegramID) {

            string msg = String.Empty;

            try {

                Account userAcc;

                /// Retrieve user object from Account using hashed TelegramUUID
                userAcc = AuthManager.Users[telegramID];
                if(userAcc == null)
                    throw new Exception("User does not exist.");

                /// IF ChatId matches what is known by server
                if (chatID.ToString().Equals(ChatId)) {

                    /// IF Telegram user's account object contains role necessary for executing this operation, OR is admin
                    foreach (string userRole in userAcc.Roles.RoleID) {
                        if (userRole.ToLower().Equals(roleID.ToLower()) || userRole.ToLower().Equals("admin"))
                            return new Tuple<bool, string>(true, null);
                    }
                }

            } catch (Exception e) {

                msg = string.Format("User failed authorization for {0} (UUID: {1}). In addition, an error was thrown: {2}",
                    roleID, telegramID, e.Message);

                Logger.Log(Status.SERVERADMIN_WARNING, msg);
                return new Tuple<bool, string>(false, msg);

            }

            msg = string.Format("User failed authorization for {0} (UUID: {1})",
                roleID, telegramID);

            Logger.Log(Status.SERVERADMIN_WARNING, msg);
            return new Tuple<bool, string>(false, msg);
        }

    }

}