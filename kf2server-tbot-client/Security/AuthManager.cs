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
            if (null == userName || null == password) {
                throw new ArgumentNullException();
            }

            if (!(userName == Properties.Settings.Default.WebAdminUsername &&
                password == Properties.Settings.Default.WebAdminPassword)) {

                throw new FaultException("Unknown Username or Password");

            }
        }

        /// <summary>
        /// Authorizer for service methods. Checks that a user can execute a method based on assigned roles in config.
        /// </summary>
        /// <param name="roleID">RoleID of service method</param>
        /// <param name="context">OperationContext of service</param>
        /// <returns>True if user is authorized, else false.</returns>
        public static bool Authorize(string roleID, OperationContext context) {

            RequestContext requestContext = context.RequestContext;
            MessageHeaders headers = requestContext.RequestMessage.Headers;
            string TelegramID = headers.GetHeader<string>(headers.FindHeader("TelegramID", ""));

            Console.WriteLine("{0}", AuthManager.Users[TelegramID]);


            if (!TelegramID.Equals("1234")) {
                return false;
            }

            return true;
        }

    }

}