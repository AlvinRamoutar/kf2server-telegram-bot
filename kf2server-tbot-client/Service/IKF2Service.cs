using kf2server_tbot_client.Utils;
using System.ServiceModel;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot_client.Service {

    /// <summary>
    /// Service methods interface
    /// </summary>
    [ServiceContract]
    public interface IKF2Service {

        #region Current Game

        [OperationContract]
        ResponseValue Status();


        [OperationContract]
        ResponseValue ChangeGameType(string gametype);

        [OperationContract]
        ResponseValue ChangeMap(string map);

        [OperationContract]
        ResponseValue ChangeGametypeAndMap(string gametype, string map);


        [OperationContract]
        ResponseValue Online();

        [OperationContract]
        ResponseValue Kick(string playername);

        #endregion

        #region Access Policy

        /// <summary>
        /// Sets an active game password for players joining.
        /// <para>If no parameter passed, then take from config (DefaultGamePassword)</para>
        /// <para>If config (DefaultGamePassword) is empty or null, then no password set (Open)</para>
        /// </summary>
        /// <param name="pwd">Game Password</param>
        /// <returns>ResponseValue object</returns>
        [OperationContract]
        ResponseValue GamePasswordOn(string pwd = null);

        /// <summary>
        /// Removes an active game password.
        /// </summary>
        /// <returns>ResponseValue object</returns>
        [OperationContract]
        ResponseValue GamePasswordOff();

        #endregion

        #region Settings

        /// <summary>
        /// Change Game Difficulty service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <returns>ResponseValue object</returns>
        [OperationContract]
        ResponseValue GameDifficulty(string difficulty);


        /// <summary>
        /// Performs a change in game length.
        /// <para>Check if length is an int (Key) or string (Value). Then, check if it exists in local dict. 
        ///  Finally, apply to new game session via map change.</para>
        /// </summary>
        /// <param name="rawLength">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>Tuple(bool:'True if successful, else false', str
        [OperationContract]
        ResponseValue GameLength(string length);


        /// <summary>
        /// Performs a change in both difficulty and game length.
        /// <para>Combination of ChangeGameDifficulty logic (<see cref="ChangeGameDifficulty(string)"/>) and
        ///  ChangeGameLength logic (<see cref="ChangeGameLength(string)"/>)</para>
        /// </summary>
        /// <param name="rawDifficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <param name="rawLength">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
        [OperationContract]
        ResponseValue GameDifficultyAndLength(string difficulty, string length);

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Submits a message through Chat Console directly to game under user 'admin'
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>ResponseValue object</returns>
        [OperationContract]
        ResponseValue AdminSay(string message);

        [OperationContract]
        ResponseValue Pause();

        [OperationContract]
        ResponseValue AddUser(string telegramUUID, string[] roles = null);

        [OperationContract]
        ResponseValue RemoveUser(string telegramUUID, string[] roles = null);

        [OperationContract]
        ResponseValue Test();

        [OperationContract]
        ResponseValue Setup(string telegramUUID, string chatId);

        #endregion

    }
}