using kf2server_tbot_client.Utils;
using System;

namespace kf2server_tbot_client.Service {

    public class AccessPolicyService : IAccessPolicyService {

        [ServiceMethodRoleID("AccessPolicy.GamePasswordOff")]
        public ResponseValue GamePasswordOff() {
            throw new NotImplementedException();
        }


        [ServiceMethodRoleID("AccessPolicy.GamePasswordOn")]
        public ResponseValue GamePasswordOn(string pwd = null) {
            throw new NotImplementedException();
        }
    }
}
