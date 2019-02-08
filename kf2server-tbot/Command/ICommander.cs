using kf2server_tbot.Utils;


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
    public interface ICommander {

        #region Current Game

        /// <summary>
        /// Returns summary of ServerAdmin/serverinfo page
        /// <para>Calls <see cref="ServerAdmin.CurrentGame.ServerInfo.Status"/></para>
        /// </summary>
        /// <returns>ResponseValue consisting of serverinfo page summary data dictionary</returns>
        ResponseValue Status();

        /// <summary>
        /// Changes gametype to specified in cmdRequest
        /// <para>Calls <see cref="ServerAdmin.CurrentGame.ChangeMap.ChangeGameTypeOnly(string)"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue ChangeGameType(CMDRequest cmdRequest);

        /// <summary>
        /// Changes map to specified in cmdRequest
        /// <para>Calls <see cref="ServerAdmin.CurrentGame.ChangeMap.ChangeMapOnly(string)"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue ChangeMap(CMDRequest cmdRequest);

        /// <summary>
        /// Changes both gametype and map to specified in cmdRequest
        /// <para>Calls <see cref="ServerAdmin.CurrentGame.ChangeMap.ChangeMapAndGameType(string, string)"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue ChangeGametypeAndMap(CMDRequest cmdRequest);

        /// <summary>
        /// Returns a listing of online players by in-game name
        /// <para>Calls <see cref="ServerAdmin.CurrentGame.Players.Online"/></para>
        /// </summary>
        /// <returns>ResponseValue object containing online users collection</returns>
        ResponseValue Online();

        /// <summary>
        /// Kicks player(s) from game with the specified in-game name
        /// <para>Calls <see cref="ServerAdmin.CurrentGame.Players.Kick(string)"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue Kick(CMDRequest cmdRequest);

        #endregion

        #region Access Policy

        /// <summary>
        /// Sets an active game password for players joining.
        /// <para>If no parameter passed, then take from config (DefaultGamePassword)</para>
        /// <para>If config (DefaultGamePassword) is empty or null, then no password set (Open)</para>
        /// <para>Calls <see cref="ServerAdmin.AccessPolicy.Passwords.GamePwd(string)"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue GamePasswordOn(CMDRequest cmdRequest);

        /// <summary>
        /// Removes active game password.
        /// <para>Calls <see cref="ServerAdmin.AccessPolicy.Passwords.GamePwd(string)"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue GamePasswordOff(CMDRequest cmdRequest);

        #endregion

        #region Settings

        /// <summary>
        /// Changes game difficulty to specified.
        /// <para>Calls <see cref="ServerAdmin.Settings.General.ChangeGameDifficulty(string)"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue GameDifficulty(CMDRequest cmdRequest);

        /// <summary>
        /// Changes game length to specified.
        /// <para>Calls <see cref="ServerAdmin.Settings.General.ChangeGameLength(string)"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue GameLength(CMDRequest cmdRequest);


        /// <summary>
        /// Performs a change in both difficulty and game length.
        /// <para>Calls <see cref="ServerAdmin.Settings.General.ChangeGameDifficultyAndLength(string, string)"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue GameDifficultyAndLength(CMDRequest cmdRequest);

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Submits a message through Chat Console directly to game under context 'admin'
        /// <para>Calls <see cref="ServerAdmin.ChatConsole.SendMessage(string)"/></para>
        /// </summary>
        /// <param name="message">CMDRequest object</param>
        /// <returns>ResponseValue consisting of message broadcasted to game session</returns>
        ResponseValue AdminSay(CMDRequest cmdRequest);

        /// <summary>
        /// WIP
        /// Pauses/Unpauses game, requires a timer to unpause after X time
        /// </summary>
        /// <param name="cmdRequest"></param>
        /// <returns></returns>
        ResponseValue Pause(CMDRequest cmdRequest);

        /// <summary>
        /// Adds mentioned user roles, or user themself, to known users file
        /// <para>Calls <see cref="Security.Users.AddUser(Telegram.Bot.Types.User, string[])"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <param name="mentionedUser">Users object of mentioned user to opt in</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue AddUser(CMDRequest cmdRequest, Telegram.Bot.Types.User mentionedUser);

        /// <summary>
        /// Removes mentioned user's roles, or user themselves from known users file
        /// <para>Calls <see cref="Security.Users.RemoveUser(Telegram.Bot.Types.User, string[])"/></para>
        /// </summary>
        /// <param name="cmdRequest">CMDRequest object</param>
        /// <param name="mentionedUser">Users object of mentioned user to demote</param>
        /// <returns>ResponseValue consisting of generic success message</returns>
        ResponseValue RemoveUser(CMDRequest cmdRequest, Telegram.Bot.Types.User mentionedUser);

        /// <summary>
        /// Ping method
        /// </summary>
        /// <returns>Pong</returns>
        ResponseValue Test();

        #endregion

    }
}