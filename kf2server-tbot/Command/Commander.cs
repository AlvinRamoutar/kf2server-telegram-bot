using kf2server_tbot.Security;
using kf2server_tbot.ServerAdmin;
using kf2server_tbot.ServerAdmin.AccessPolicy;
using kf2server_tbot.ServerAdmin.CurrentGame;
using kf2server_tbot.ServerAdmin.Settings;
using kf2server_tbot.Utils;
using LogEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.Command {

    /// <summary>
    /// Command Implementation
    /// <para>Performs queued calls to ServerAdmin operation methods</para>
    /// <para>Almost all methods follow this schema:</para>
    /// <para>1. Have the 'CommandRoleID' attribute, which must be in Users.cs, and
    ///  is used to authorization</para>
    /// <para>2. Perform a call to AuthManager.Authorize, which supplies the method's 'ServiceMethodRoleID'
    ///  attribute, as well as current OperationContext object (to grab user info from HTTP headers) </para>
    /// <para>3. Check the result of call from 2., and perform calls to necessary ServerAdmin operations
    ///  within a thread supplied to ActionQueue.Act (encapsulates for queue)</para>
    /// <para>4. Log the call of this service method using LogEngine</para>
    /// </summary>
    class Commander : ICommander {

        #region Current Game

        [RoleID("CurrentGame.ChangeGameType")]
        public ResponseValue ChangeGameType(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeGameType").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeGameTypeOnly(cmdRequest.Args[0]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("ChangeGameType").GetCustomAttributes(true)
                        .OfType<RoleIDAttribute>().FirstOrDefault().ID,
                        cmdRequest.Args[0]));

                return new ResponseValue(true, string.Format("Changing game type to: {0}", cmdRequest.Args[0]), null);

            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


        [RoleID("CurrentGame.ChangeGametypeAndMap")]
        public ResponseValue ChangeGametypeAndMap(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeGametypeAndMap").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeMapAndGameType(cmdRequest.Args[0], cmdRequest.Args[1]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}', '{2}')", GetType().GetMethod("ChangeGametypeAndMap").GetCustomAttributes(true)
                        .OfType<RoleIDAttribute>().FirstOrDefault().ID,
                        cmdRequest.Args[0], cmdRequest.Args[1]));

                return new ResponseValue(true, string.Format("Changing game type to: {0} and map to: {1}", 
                    cmdRequest.Args[0], cmdRequest.Args[1]), null);

            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


        [RoleID("CurrentGame.ChangeMap")]
        public ResponseValue ChangeMap(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeMap").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeMapOnly(cmdRequest.Args[0]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("ChangeMap").GetCustomAttributes(true)
                        .OfType<RoleIDAttribute>().FirstOrDefault().ID,
                        cmdRequest.Args[0]));

                return new ResponseValue(true, string.Format("Changing map to: {0}", cmdRequest.Args[0]), null);

            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


        [RoleID("CurrentGame.Online")]
        public ResponseValue Online() {

            System.Threading.ManualResetEvent syncEvent = new System.Threading.ManualResetEvent(false);

            Tuple<bool, string> Result = new Tuple<bool, string>(false, null);

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                Result = Players.Instance.Online();
                syncEvent.Set();
            }));

            syncEvent.WaitOne();

            if (!Result.Item1) {

                Logger.Log(LogEngine.Status.SERVERADMIN_FAILURE, "Failed to retrieve player info from Players");

                return new ResponseValue(false, null, null);

            }
            else {

                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0}", GetType().GetMethod("Online")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID));

                return new ResponseValue(true, string.Empty, new Dictionary<string, string>() { { "online", Result.Item2 } });
            }

        }


        [RoleID("CurrentGame.Kick")]
        public ResponseValue Kick(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("Kick").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    Players.Instance.Kick(cmdRequest.Args[0]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("Kick").GetCustomAttributes(true)
                        .OfType<RoleIDAttribute>().FirstOrDefault().ID,
                        cmdRequest.Args[0]));

                return new ResponseValue(true, string.Format("Kicking player: {0}", cmdRequest.Args[0]), null);

            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


        [RoleID("CurrentGame.Status")]
        public ResponseValue Status() {

            System.Threading.ManualResetEvent syncEvent = new System.Threading.ManualResetEvent(false);

            Tuple<bool, Dictionary<string, string>> Result = new Tuple<bool, Dictionary<string, string>>(false, null);

            ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                Result = ServerInfo.Instance.Status();
                syncEvent.Set();
            }));

            syncEvent.WaitOne();

            if (!Result.Item1) {

                Logger.Log(LogEngine.Status.SERVERADMIN_FAILURE, "Failed to retrieve status information from ServerInfo");

                return new ResponseValue(false, null, null);

            }
            else {

                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0}", GetType().GetMethod("Status")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID));

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
        [RoleID("AccessPolicy.GamePasswordOn")]
        public ResponseValue GamePasswordOn(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GamePasswordOn").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    Passwords.Instance.GamePwd((cmdRequest.Args.Length == 0) ? Properties.Settings.Default.DefaultGamePassword : cmdRequest.Args[0]);
                }));

                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("GamePasswordOn").
                    GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    (cmdRequest.Args.Length == 0) ? Properties.Settings.Default.DefaultGamePassword : cmdRequest.Args[0]));

                return new ResponseValue(true, string.Format("Changing game password"), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }
        }


        /// <summary>
        /// Removes an active game password.
        /// </summary>
        /// <returns>ResponseValue object</returns>
        [RoleID("AccessPolicy.GamePasswordOff")]
        public ResponseValue GamePasswordOff(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GamePasswordOff").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    Passwords.Instance.GamePwd();
                }));

                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0}", GetType().GetMethod("GamePasswordOff").
                    GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID));

                return new ResponseValue(true, string.Format("Removing game password"), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }
        }

        #endregion

        #region Settings

        /// <summary>
        /// Change Game Difficulty service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <returns>ResponseValue object</returns>
        [RoleID("Settings.GameDifficulty")]
        public ResponseValue GameDifficulty(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameDifficulty").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameDifficulty(cmdRequest.Args[0]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("GameDifficulty")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    cmdRequest.Args[0]));

                return new ResponseValue(true, string.Format("Changing game difficulty to: {0}", cmdRequest.Args[0]), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


        /// <summary>
        /// Change Game Length service method
        /// </summary>
        /// <param name="length">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>ResponseValue object</returns>
        [RoleID("Settings.GameLength")]
        public ResponseValue GameLength(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameLength").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameLength(cmdRequest.Args[0]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("GameLength")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    cmdRequest.Args[0]));


                return new ResponseValue(true, string.Format("Changing game length to: {0}", cmdRequest.Args[0]), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


        /// <summary>
        /// Change Game Difficulty and Length service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <param name="length">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>ResponseValue object</returns>
        [RoleID("Settings.GameDifficultyAndLength")]
        public ResponseValue GameDifficultyAndLength(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameDifficultyAndLength").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameDifficultyAndLength(cmdRequest.Args[0], cmdRequest.Args[1]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}, {2}')", GetType().GetMethod("GameDifficultyAndLength")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    cmdRequest.Args[0], cmdRequest.Args[1]));

                return new ResponseValue(true, string.Format("Changing game difficulty to: {0} and length to: {1}", 
                    cmdRequest.Args[0], cmdRequest.Args[1]), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }
        }

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Submits a message through Chat Console directly to game under user 'admin'
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>ResponseValue object</returns>
        [RoleID("Miscellaneous.AdminSay")]
        public ResponseValue AdminSay(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("AdminSay").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);

            string builtMsg = "";
            foreach(string part in cmdRequest.Args.Skip(1)) {
                builtMsg += " " + part;
            }

            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ChatConsole.SendMessage(builtMsg);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("AdminSay").GetCustomAttributes(true)
                        .OfType<RoleIDAttribute>().FirstOrDefault().ID,
                        builtMsg));

                return new ResponseValue(true, builtMsg, null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }
        }


        /// <summary>
        /// WIP
        /// Not implemented, since there is no known way to pause through server admin
        /// (pause is not a supported command in Management Console)
        /// </summary>
        /// <returns></returns>
        [RoleID("Miscellaneous.Pause")]
        public ResponseValue Pause(CMDRequest cmdRequest) {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Adds a specified Telegram user to the list of known users. Also assigns supplied roles
        /// </summary>
        /// <param name="telegramUUID">Telegram UUID</param>
        /// <param name="roles">Array of roles</param>
        /// <returns>ResponseValue object</returns>
        [RoleID("Miscellaneous.AddUser")]
        public ResponseValue AddUser(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("AddUser").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);

            /// Confirms if user is authorized, OR if we're overriding, designated by a UUID of 777
            if (AuthResult.Item1 || cmdRequest.UserID == 777) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    try {
                        Users.AddUser(AuthManager.Users, long.Parse(cmdRequest.Args[1]), cmdRequest.Args.Skip(2).ToArray());
                    }
                    catch (Exception) { }
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}', '{2}')", GetType().GetMethod("AddUser")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    cmdRequest.UserID, JsonConvert.SerializeObject(cmdRequest.Args)));


                return new ResponseValue(true, "New User Added", new Dictionary<string, string>() {
                    { "roles", (cmdRequest.Args != null) ? JsonConvert.SerializeObject(cmdRequest.Args) : string.Empty }
                });


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }



        /// <summary>
        /// Removes a specified Telegram user from list of known users, or updates their roles
        /// </summary>
        /// <param name="telegramUUID">Telegram UUID</param>
        /// <param name="roles">Array of roles</param>
        /// <returns>ResponseValue object</returns>
        [RoleID("MiscellaneousService.RemoveUser")]
        public ResponseValue RemoveUser(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("RemoveUser").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.UserID);

            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    try {
                        Users.RemoveUser(AuthManager.Users, cmdRequest.UserID, cmdRequest.Args);
                    }
                    catch (Exception) { }
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}', '{2}')", GetType().GetMethod("RemoveUser")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    cmdRequest.UserID, JsonConvert.SerializeObject(cmdRequest.Args)));


                return new ResponseValue(true, "User or roles were removed", new Dictionary<string, string>() {
                    { "roles", (cmdRequest.Args != null) ? JsonConvert.SerializeObject(cmdRequest.Args) : string.Empty }
                });


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


        /// <summary>
        /// Test service method, use for debugging auth or something.
        /// </summary>
        /// <returns></returns>
        [RoleID("Miscellaneous.Test")]
        public ResponseValue Test() {

            Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                string.Format("{0}", GetType().GetMethod("Test")
                .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID));

            return new ResponseValue(true, "The pong to your ping",
                new Dictionary<string, string>() { { "errors", null } });

        }

        #endregion

    }
}
