using kf2server_tbot_client.Utils;
using System;
using System.Collections.Generic;

namespace kf2server_tbot_client.Service {

    public class CurrentGameService : ICurrentGameService {

        [ServiceMethodRoleID("CurrentGame.ChangeGameType")]
        public ResponseValue ChangeGameType(string gametype) {
            throw new NotImplementedException();
        }


        [ServiceMethodRoleID("CurrentGame.ChangeGametypeAndMap")]
        public ResponseValue ChangeGametypeAndMap(string gametype, string map) {
            throw new NotImplementedException();
        }


        [ServiceMethodRoleID("CurrentGame.ChangeMap")]
        public ResponseValue ChangeMap(string map) {
            throw new NotImplementedException();
        }


        [ServiceMethodRoleID("CurrentGame.Players")]
        public ResponseValue Players() {
            throw new NotImplementedException();
        }


        [ServiceMethodRoleID("CurrentGame.Players_KickPlayer")]
        public ResponseValue Players_KickPlayer() {
            throw new NotImplementedException();
        }


        [ServiceMethodRoleID("CurrentGame.Status")]
        public ResponseValue Status() {
            throw new NotImplementedException();
        }
    }
}
