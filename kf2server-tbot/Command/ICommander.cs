using kf2server_tbot.Utils;


/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot.Command {

    /// <summary>
    /// KF2 ServerAdmin Actions
    /// </summary>
    public interface ICommander {

        #region Current Game

        
        ResponseValue Status();
        
        ResponseValue ChangeGameType(CMDRequest cmdRequest);

        
        ResponseValue ChangeMap(CMDRequest cmdRequest);

        
        ResponseValue ChangeGametypeAndMap(CMDRequest cmdRequest);


        
        ResponseValue Online();

        
        ResponseValue Kick(CMDRequest cmdRequest);

        #endregion

        #region Access Policy

        /// <summary>
        /// Sets an active game password for players joining.
        /// <para>If no parameter passed, then take from config (DefaultGamePassword)</para>
        /// <para>If config (DefaultGamePassword) is empty or null, then no password set (Open)</para>
        /// </summary>
        /// <param name="pwd">Game Password</param>
        /// <returns>ResponseValue object</returns>
        
        ResponseValue GamePasswordOn(CMDRequest cmdRequest);

        /// <summary>
        /// Removes an active game password.
        /// </summary>
        /// <returns>ResponseValue object</returns>
        
        ResponseValue GamePasswordOff(CMDRequest cmdRequest);

        #endregion

        #region Settings

        /// <summary>
        /// Change Game Difficulty service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <returns>ResponseValue object</returns>
        
        ResponseValue GameDifficulty(CMDRequest cmdRequest);


        /// <summary>
        /// Performs a change in game length.
        /// <para>Check if length is an int (Key) or string (Value). Then, check if it exists in local dict. 
        ///  Finally, apply to new game session via map change.</para>
        /// </summary>
        /// <param name="rawLength">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>Tuple(bool:'True if successful, else false', str
        
        ResponseValue GameLength(CMDRequest cmdRequest);


        /// <summary>
        /// Performs a change in both difficulty and game length.
        /// <para>Combination of ChangeGameDifficulty logic (<see cref="ChangeGameDifficulty(CMDRequest)"/>) and
        ///  ChangeGameLength logic (<see cref="ChangeGameLength(CMDRequest)"/>)</para>
        /// </summary>
        /// <param name="rawDifficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <param name="rawLength">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
        
        ResponseValue GameDifficultyAndLength(CMDRequest cmdRequest);

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Submits a message through Chat Console directly to game under user 'admin'
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>ResponseValue object</returns>
        
        ResponseValue AdminSay(CMDRequest cmdRequest);

        
        ResponseValue Pause(CMDRequest cmdRequest);


        ResponseValue AddUser(CMDRequest cmdRequest, Telegram.Bot.Types.User mentionedUser);

        
        ResponseValue RemoveUser(CMDRequest cmdRequest, Telegram.Bot.Types.User mentionedUser);

        
        ResponseValue Test();

        #endregion

    }
}