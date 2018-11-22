using System;
using System.IdentityModel.Selectors;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace kf2server_tbot_client.Security {

    /// <summary>
    /// Simple custom Authorization and Authentication
    /// </summary>
    public class AuthManager : UserNamePasswordValidator {

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
        /// Validator for service authorization. Authorizes against Service credentials from config.
        /// </summary>
        /// <param name="userName">ServiceUsername</param>
        /// <param name="password">ServicePassword</param>
        public override void Validate(string userName, string password) {

            if (null == userName || null == password) {
                throw new ArgumentNullException();
            }

            if (!(userName == Properties.Settings.Default.ServiceUsername &&
                password == Properties.Settings.Default.ServicePassword)) {

                throw new FaultException("Unknown Username or Password");

            }
        }


        /// <summary>
        /// WIP
        /// Authorizer for service methods. Checks that a user can execute a method based on assigned roles in config.
        /// </summary>
        /// <param name="roleID">RoleID of service method</param>
        /// <param name="context">OperationContext of service</param>
        /// <returns>True if user is authorized, else false.</returns>
        public static Tuple<bool, string> Authorize(string roleID, OperationContext context) {

            string TelegramUUID = string.Empty;
            string _ChatId = string.Empty;

            try {

                RequestContext requestContext = context.RequestContext;
                MessageHeaders headers = requestContext.RequestMessage.Headers;

                TelegramUUID = headers.GetHeader<string>(headers.FindHeader("TelegramID", ""));
                _ChatId = headers.GetHeader<string>(headers.FindHeader("ChatId", ""));

                Account userAcc = AuthManager.Users[Crypto.Hash(TelegramUUID)];


                if (_ChatId.Equals(ChatId)) {

                    foreach (string userRole in userAcc.Roles.RoleID) {
                        if (userRole.ToLower().Equals(roleID.ToLower()))
                            return new Tuple<bool, string>(true, null);
                    }
                }

            } catch (Exception e) {
                return new Tuple<bool, string>(false, string.Format("User failed authorization for {0} (UUID: {1}). In addition, an error was thrown: {2}",
                    roleID, (string.IsNullOrEmpty(TelegramUUID)) ? "Undefined" : Crypto.Hash(TelegramUUID), e.Message));
            }

            return new Tuple<bool, string>(false, string.Format("User failed authorization for {0} (UUID: {1})",
                roleID, (string.IsNullOrEmpty(TelegramUUID)) ? "Undefined" : Crypto.Hash(TelegramUUID)));
        }

    }

}