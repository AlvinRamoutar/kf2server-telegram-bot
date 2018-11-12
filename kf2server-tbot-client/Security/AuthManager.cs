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
        /// Validator for service authorization. Authorizes against KF2 Server Webmin credentials from config.
        /// </summary>
        /// <param name="userName">KF2 Server Webmin Username</param>
        /// <param name="password">KF2 Server Webmin Password</param>
        public override void Validate(string userName, string password) {

            Console.WriteLine("Authing for {0} {1}", userName, password);

            if (null == userName || null == password) {
                throw new ArgumentNullException();
            }

            if (!(userName == Properties.Settings.Default.WebAdminUsername &&
                password == Properties.Settings.Default.WebAdminPassword)) {

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
        public static bool Authorize(string roleID, OperationContext context) {

            try {

                RequestContext requestContext = context.RequestContext;
                MessageHeaders headers = requestContext.RequestMessage.Headers;

                string TelegramUUID = headers.GetHeader<string>(headers.FindHeader("TelegramID", ""));

                Console.WriteLine("Trying auth for: {0}", TelegramUUID);

                Account userAcc = AuthManager.Users[Crypto.Hash(TelegramUUID)];

                foreach (string userRole in userAcc.Roles.RoleID) {
                    if (userRole.Equals(roleID))
                        return true;
                }

            } catch (Exception) { }

            return false;
        }

    }

}