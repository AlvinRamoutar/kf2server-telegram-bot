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
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.Command {

    /// <summary>
    /// Command Handler
    /// <para>Performs queued calls to ServerAdmin operation methods</para>
    /// <para>Almost all methods follow this schema:</para>
    /// <para>1. Have the 'CommandRoleID' attribute which identifies this method, which must
    ///  be defined in Users.cs, and is used for authorization</para>
    /// <para>2. Perform a call to AuthManager.Authorize, which supplies the method's 'ServiceMethodRoleID'</para>
    /// <para>3. Check the result of call from previous, and perform calls to necessary ServerAdmin operations
    ///  within a thread supplied to ActionQueue.Act (encapsulated for queue)</para>
    /// <para>4. Log the call of this service method using LogEngine</para>
    /// </summary>
    class Commander : ICommander {

        #region Current Game

        [RoleID("CurrentGame.ChangeGameType")]
        public ResponseValue ChangeGameType(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("ChangeGameType").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.User.Id);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeGameTypeOnly(cmdRequest.Args[1]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("ChangeGameType").GetCustomAttributes(true)
                        .OfType<RoleIDAttribute>().FirstOrDefault().ID,
                        cmdRequest.Args[1]));

                return new ResponseValue(true, string.Format("Changing game type to: {0}", cmdRequest.Args[1]), null);

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
                cmdRequest.ChatID, cmdRequest.User.Id);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeMapAndGameType(cmdRequest.Args[1], cmdRequest.Args[2]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}', '{2}')", GetType().GetMethod("ChangeGametypeAndMap").GetCustomAttributes(true)
                        .OfType<RoleIDAttribute>().FirstOrDefault().ID,
                        cmdRequest.Args[1], cmdRequest.Args[2]));

                return new ResponseValue(true, string.Format("Changing gametype to: {0} and map to: {1}", 
                    cmdRequest.Args[1], cmdRequest.Args[2]), null);

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
                cmdRequest.ChatID, cmdRequest.User.Id);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    ServerAdmin.CurrentGame.ChangeMap.Instance.ChangeMapOnly(cmdRequest.Args[1]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("ChangeMap").GetCustomAttributes(true)
                        .OfType<RoleIDAttribute>().FirstOrDefault().ID,
                        cmdRequest.Args[1]));

                return new ResponseValue(true, string.Format("Changing map to: {0}", cmdRequest.Args[1]), null);

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
                cmdRequest.ChatID, cmdRequest.User.Id);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    Players.Instance.Kick(cmdRequest.Args[1]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("Kick").GetCustomAttributes(true)
                        .OfType<RoleIDAttribute>().FirstOrDefault().ID,
                        cmdRequest.Args[1]));

                return new ResponseValue(true, string.Format("Kicking player: {0}", cmdRequest.Args[1]), null);

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

        [RoleID("AccessPolicy.GamePasswordOn")]
        public ResponseValue GamePasswordOn(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GamePasswordOn").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.User.Id);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    Passwords.Instance.GamePwd((cmdRequest.Args.Length == 1) ? Properties.Settings.Default.DefaultGamePassword : cmdRequest.Args[1]);
                }));

                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("GamePasswordOn").
                    GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    (cmdRequest.Args.Length == 0) ? Properties.Settings.Default.DefaultGamePassword : cmdRequest.Args[1]));

                return new ResponseValue(true, string.Format("Changing game password"), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }
        }


        [RoleID("AccessPolicy.GamePasswordOff")]
        public ResponseValue GamePasswordOff(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GamePasswordOff").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.User.Id);


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

        [RoleID("Settings.GameDifficulty")]
        public ResponseValue GameDifficulty(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameDifficulty").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.User.Id);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameDifficulty(cmdRequest.Args[1]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("GameDifficulty")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    cmdRequest.Args[1]));

                return new ResponseValue(true, string.Format("Changing game difficulty to: {0}", cmdRequest.Args[1]), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }

        [RoleID("Settings.GameLength")]
        public ResponseValue GameLength(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameLength").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.User.Id);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameLength(cmdRequest.Args[1]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}')", GetType().GetMethod("GameLength")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    cmdRequest.Args[1]));


                return new ResponseValue(true, string.Format("Changing game length to: {0}", cmdRequest.Args[1]), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


        [RoleID("Settings.GameDifficultyAndLength")]
        public ResponseValue GameDifficultyAndLength(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("GameDifficultyAndLength").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.User.Id);


            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    General.Instance.ChangeGameDifficultyAndLength(cmdRequest.Args[1], cmdRequest.Args[2]);
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}, {2}')", GetType().GetMethod("GameDifficultyAndLength")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    cmdRequest.Args[1], cmdRequest.Args[2]));

                return new ResponseValue(true, string.Format("Changing game difficulty to: {0} and length to: {1}", 
                    cmdRequest.Args[1], cmdRequest.Args[2]), null);


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }
        }

        #endregion

        #region Miscellaneous

        [RoleID("Miscellaneous.AdminSay")]
        public ResponseValue AdminSay(CMDRequest cmdRequest) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("AdminSay").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.User.Id);

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


        [RoleID("Miscellaneous.Pause")]
        public ResponseValue Pause(CMDRequest cmdRequest) {
            throw new NotImplementedException();
        }


        [RoleID("Miscellaneous.AddUser")]
        public ResponseValue AddUser(CMDRequest cmdRequest, Telegram.Bot.Types.User mentionedUser) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("AddUser").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.User.Id);

            /// Confirms if user is authorized, or part of the setup process
            if (AuthResult.Item1 || 
                ((cmdRequest.Data == null) ? false : cmdRequest.Data.ToString().Equals("setup"))) {

              
                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    try {
                        Users.AddUser(mentionedUser, cmdRequest.Args.Skip(2).ToArray());
                    }
                    catch (Exception) { }
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}', '{2}')", GetType().GetMethod("AddUser")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    cmdRequest.Args[1], JsonConvert.SerializeObject(cmdRequest.Args.Skip<string>(2))));


                return new ResponseValue(true, "New User Added", new Dictionary<string, string>() {
                    { "roles", (cmdRequest.Args != null) ? JsonConvert.SerializeObject(cmdRequest.Args) : string.Empty }
                });

            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


        [RoleID("MiscellaneousService.RemoveUser")]
        public ResponseValue RemoveUser(CMDRequest cmdRequest, Telegram.Bot.Types.User mentionedUser) {

            Tuple<bool, string> AuthResult = AuthManager.Authorize(
                GetType().GetMethod("RemoveUser").GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                cmdRequest.ChatID, cmdRequest.User.Id);

            if (AuthResult.Item1) {

                ActionQueue.Instance.Act(new System.Threading.Thread(() => {
                    try {
                        Users.RemoveUser(mentionedUser, cmdRequest.Args);
                    }
                    catch (Exception) { }
                }));


                Logger.Log(LogEngine.Status.SERVERADMIN_INFO,
                    string.Format("{0} ('{1}', '{2}')", GetType().GetMethod("RemoveUser")
                    .GetCustomAttributes(true).OfType<RoleIDAttribute>().FirstOrDefault().ID,
                    mentionedUser.Id, JsonConvert.SerializeObject(cmdRequest.Args)));


                return new ResponseValue(true, "User or roles were removed", new Dictionary<string, string>() {
                    { "roles", (cmdRequest.Args != null) ? JsonConvert.SerializeObject(cmdRequest.Args) : string.Empty }
                });


            }
            else {
                Logger.Log(LogEngine.Status.SERVERADMIN_WARNING, AuthResult.Item2);

                return new ResponseValue(false, "You don't have the privilege to perform this action.", null);
            }

        }


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
