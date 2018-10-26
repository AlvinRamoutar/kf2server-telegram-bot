using System;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Channels;
using TWCFClient.Utils;

namespace TWCFClient.Service {

    public class MiscellaneousService : IMiscellaneousService {

        [ServiceMethodRoleID("Miscellaneous.AdminSay")]
        public ResponseValue AdminSay(string message) {
            throw new NotImplementedException();
        }

        [ServiceMethodRoleID("Miscellaneous.Pause")]
        public ResponseValue Pause() {
            throw new NotImplementedException();
        }

        [ServiceMethodRoleID("Miscellaneous.Test")]
        public ResponseValue Test() {

            if (Auth.AuthManager.Authorize(
                GetType().GetMethod("Test").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current)) {

                return new ResponseValue(true, Auth.Crypto.Hash("This is my Telegram ID."), 
                    new System.Collections.Generic.Dictionary<string, string>() { { "errors", null } });

            } else {
                throw new FaultException("You don't have the privilege to perform this action.");
            }
            
        }

    }
}
