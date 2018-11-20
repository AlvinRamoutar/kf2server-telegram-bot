using kf2server_tbot_client.Security;
using kf2server_tbot_client.ServerAdmin;
using kf2server_tbot_client.ServerAdmin.AccessPolicy;
using kf2server_tbot_client.ServerAdmin.CurrentGame;
using kf2server_tbot_client.ServerAdmin.Settings;
using kf2server_tbot_client.Utils;
using LogEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {
    class KF2Service : ServiceTools, IKF2Service {

        #region Current Game

        [ServiceMethodRoleID("CurrentGame.ChangeGameType")]
        public ResponseValue ChangeGameType(string gametype) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeGameType").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeGameTypeOnly(gametype);
                }));


                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}')", GetType().GetMethod("ChangeGameType").GetCustomAttributes(true)
                        .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), gametype));

                return new ResponseValue(true, string.Format("Changing game type to: {0}", gametype), null);

            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        [ServiceMethodRoleID("CurrentGame.ChangeGametypeAndMap")]
        public ResponseValue ChangeGametypeAndMap(string gametype, string map) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeGametypeAndMap").GetCustomAttributes(true)
                .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeMapAndGameType(gametype, map);
                }));


                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}', '{3}')", GetType().GetMethod("ChangeGametypeAndMap").GetCustomAttributes(true)
                        .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), gametype, map));

                return new ResponseValue(true, string.Format("Changing game type to: {0} and map to: {1}", gametype, map), null);

            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        [ServiceMethodRoleID("CurrentGame.ChangeMap")]
        public ResponseValue ChangeMap(string map) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeMap").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeMapOnly(map);
                }));


                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}')", GetType().GetMethod("ChangeMap").GetCustomAttributes(true)
                        .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), map));

                return new ResponseValue(true, string.Format("Changing map to: {0}", map), null);

            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        [ServiceMethodRoleID("CurrentGame.Online")]
        public ResponseValue Online() {

            System.Threading.ManualResetEvent syncEvent = new System.Threading.ManualResetEvent(false);

            Tuple<bool, string> Result = new Tuple<bool, string>(false, null);

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                Result = Players.Instance.Online();
                syncEvent.Set();
            }));

            syncEvent.WaitOne();

            if (!Result.Item1) {

                Logger.Log(LogEngine.Status.SERVICE_FAILURE, "Failed to retrieve player info from Players");

                return new ResponseValue(false, null, null);

            }
            else {

                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} from {1}", GetType().GetMethod("Online")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, GetIP()));

                return new ResponseValue(true, string.Empty, new Dictionary<string, string>() { { "online", Result.Item2 } });
            }

        }


        [ServiceMethodRoleID("CurrentGame.Kick")]
        public ResponseValue Kick(string playername) {

            //Tuple<bool, string> AuthResult = AuthManager.Authorize(
            //    GetType().GetMethod("Kick").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
            //    OperationContext.Current);


            //if (AuthResult.Item1) {

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                ServerAdmin.CurrentGame.Players.Instance.Kick(playername);
            }));


            Logger.Log(LogEngine.Status.SERVICE_INFO,
                string.Format("{0} for {1} ('{2}')", GetType().GetMethod("Kick").GetCustomAttributes(true)
                    .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                GetIP(), playername));

            return new ResponseValue(true, string.Format("Kicking player: {0}", playername), null);

            //} else {
            //    Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

            //    throw new FaultException("You don't have the privilege to perform this action.");
            //}

        }



        [ServiceMethodRoleID("CurrentGame.Status")]
        public ResponseValue Status() {

            System.Threading.ManualResetEvent syncEvent = new System.Threading.ManualResetEvent(false);

            Tuple<bool, Dictionary<string, string>> Result = new Tuple<bool, Dictionary<string, string>>(false, null);

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                Result = ServerInfo.Instance.Status();
                syncEvent.Set();
            }));

            syncEvent.WaitOne();

            if (!Result.Item1) {

                Logger.Log(LogEngine.Status.SERVICE_FAILURE, "Failed to retrieve status information from ServerInfo");

                return new ResponseValue(false, null, null);

            }
            else {

                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} from {1}", GetType().GetMethod("Status")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, GetIP()));

                return new ResponseValue(true, string.Empty, Result.Item2);
            }
        }

        #endregion

        #region Access Policy

        /// <summary>
        /// Sets an active game password for players joining.
        /// <para>If no parameter passed, then take from config (DefaultGamePassword)</para>
        /// <para>If config (DefaultGamePassword) is empty or null, then no password set (Open)</para>
        /// </summary>
        /// <param name="pwd">Game Password</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("AccessPolicy.GamePasswordOn")]
        public ResponseValue GamePasswordOn(string pwd = null) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GamePasswordOn").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    Passwords.Instance.GamePwd((string.IsNullOrEmpty(pwd)) ? Properties.Settings.Default.DefaultGamePassword : pwd);
                }));

                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}')", GetType().GetMethod("GamePasswordOn").
                    GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), pwd));

                return new ResponseValue(true, string.Format("Setting game password to ",
                    (string.IsNullOrEmpty(pwd)) ? "'" + pwd + "'" : "DefaultGamePassword from config"), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }
        }


        /// <summary>
        /// Removes an active game password.
        /// </summary>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("AccessPolicy.GamePasswordOff")]
        public ResponseValue GamePasswordOff() {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GamePasswordOff").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    Passwords.Instance.GamePwd();
                }));

                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} for {1}", GetType().GetMethod("GamePasswordOff").
                    GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP()));

                return new ResponseValue(true, string.Format("Removing game password"), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }
        }

        #endregion

        #region Settings

        /// <summary>
        /// Change Game Difficulty service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Settings.GameDifficulty")]
        public ResponseValue GameDifficulty(string difficulty) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameDifficulty").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameDifficulty(difficulty);
                }));


                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}')", GetType().GetMethod("GameDifficulty")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), difficulty));

                return new ResponseValue(true, string.Format("Changing game difficulty to: {0}", difficulty), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        /// <summary>
        /// Change Game Length service method
        /// </summary>
        /// <param name="length">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Settings.GameLength")]
        public ResponseValue GameLength(string length) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameLength").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameLength(length);
                }));


                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}')", GetType().GetMethod("GameLength")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), length));


                return new ResponseValue(true, string.Format("Changing game length to: {0}", length), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        /// <summary>
        /// Change Game Difficulty and Length service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <param name="length">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Settings.GameDifficultyAndLength")]
        public ResponseValue GameDifficultyAndLength(string difficulty, string length) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameDifficultyAndLength").GetCustomAttributes(true)
                    .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameDifficultyAndLength(difficulty, length);
                }));


                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}, {3}')", GetType().GetMethod("GameDifficultyAndLength")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), difficulty, length));

                return new ResponseValue(true, string.Format("Changing game difficulty to: {0} and length to: {1}", difficulty, length), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }
        }

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Submits a message through Chat Console directly to game under user 'admin'
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Miscellaneous.AdminSay")]
        public ResponseValue AdminSay(string message) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("AdminSay").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ChatConsole.SendMessage(message);
                }));


                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} for {1} ('{2}')", GetType().GetMethod("AdminSay").GetCustomAttributes(true)
                        .OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), message));

                return new ResponseValue(true, message, null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }
        }



        [ServiceMethodRoleID("Miscellaneous.Pause")]
        public ResponseValue Pause() {
            throw new NotImplementedException();
        }



        [ServiceMethodRoleID("Miscellaneous.AddUser")]
        public ResponseValue AddUser(string telegramUUID, string[] roles = null) {

            /*
            Console.WriteLine(OperationContext.Current.RequestContext.RequestMessage.ToString());
            foreach(var header in OperationContext.Current.RequestContext.RequestMessage.Headers) {
                Console.WriteLine(header.ToString());
            }
            */

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("AddUser").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);

            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    try {
                        bool doesUserExist = Users.AddUser(AuthManager.Users, telegramUUID, roles);
                    }
                    catch (Exception) { }
                }));


                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}', '{3}')", GetType().GetMethod("AddUser")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), telegramUUID, JsonConvert.SerializeObject(roles)));


                return new ResponseValue(true, "New User Added", new Dictionary<string, string>() {
                    { "roles", (roles != null) ? JsonConvert.SerializeObject(roles) : string.Empty }
                });


            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }



        [ServiceMethodRoleID("MiscellaneousService.RemoveUser")]
        public ResponseValue RemoveUser(string telegramUUID, string[] roles = null) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("RemoveUser").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);

            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    try {
                        Users.RemoveUser(AuthManager.Users, telegramUUID, roles);
                    }
                    catch (Exception) { }
                }));


                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} from {1} ('{2}', '{3}')", GetType().GetMethod("RemoveUser")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                    GetIP(), telegramUUID, JsonConvert.SerializeObject(roles)));


                return new ResponseValue(true, "User or roles were removed", new Dictionary<string, string>() {
                    { "roles", (roles != null) ? JsonConvert.SerializeObject(roles) : string.Empty }
                });


            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }



        [ServiceMethodRoleID("Miscellaneous.Test")]
        public ResponseValue Test() {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("Test").GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID,
                OperationContext.Current);

            if (AuthResult.Item1) {

                Logger.Log(LogEngine.Status.SERVICE_INFO,
                    string.Format("{0} from {1}", GetType().GetMethod("Test")
                    .GetCustomAttributes(true).OfType<ServiceMethodRoleIDAttribute>().FirstOrDefault().ID, GetIP()));

                return new ResponseValue(true, "This is a sample action, occuring on successful authentication.",
                    new Dictionary<string, string>() { { "errors", null } });

            }
            else {
                Logger.Log(LogEngine.Status.SERVICE_WARNING, AuthResult.Item2);

                throw new FaultException("You don't have the privilege to perform this action.");
            }

        }


        /// <summary>
        /// Binds the supplied telegram ChatId to AuthManager.
        /// All SOAP requests must now contain this ChatId.
        /// </summary>
        /// <param name="chatId">Telegram ChatId</param>
        /// <returns>ResponseValue object</returns>
        [ServiceMethodRoleID("Miscellaneous.Setup")]
        public ResponseValue Setup(string chatId) {

            /// If none is bound, then bound.
            if (string.IsNullOrWhiteSpace(AuthManager.ChatId)) {

                AuthManager.ChatId = chatId;
                return new ResponseValue(true, chatId, new Dictionary<string, string>() { { "errors", null } });

            /// If one already exists, check if it is 
            } else if(AuthManager.ChatId.Equals(chatId)) {

                return new ResponseValue(true, chatId, new Dictionary<string, string>() { { "errors", null } });
            }

            return new ResponseValue(false, "Already bound to existing chat", new Dictionary<string, string>() { { "errors", null } });
        }

        #endregion

    }
}
