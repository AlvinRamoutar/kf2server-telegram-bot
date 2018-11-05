using kf2server_tbot_client.Utils;
using System.ServiceModel;

namespace kf2server_tbot_client.Service {

    /// <summary>
    /// Interface containing definitions of service methods for Settings category
    /// </summary>
    [ServiceContract]
    public interface ISettingsService {

        /// <summary>
        /// Change Game Difficulty service method
        /// </summary>
        /// <param name="difficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <returns>ResponseValue object</returns>
        [OperationContract]
        ResponseValue General_Game_GameDifficulty(string difficulty);


        /// <summary>
        /// Performs a change in game length.
        /// <para>Check if length is an int (Key) or string (Value). Then, check if it exists in local dict. 
        ///  Finally, apply to new game session via map change.</para>
        /// </summary>
        /// <param name="rawLength">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>Tuple(bool:'True if successful, else false', str
        [OperationContract]
        ResponseValue General_Game_GameLength(string length);


        /// <summary>
        /// Performs a change in both difficulty and game length.
        /// <para>Combination of ChangeGameDifficulty logic (<see cref="ChangeGameDifficulty(string)"/>) and
        ///  ChangeGameLength logic (<see cref="ChangeGameLength(string)"/>)</para>
        /// </summary>
        /// <param name="rawDifficulty">Difficulty, either as key (double, e.g. 1.0000), or value (text, e.g. "normal")</param>
        /// <param name="rawLength">Length, either as key (int, e.g. 1), or value (text, e.g. "short")</param>
        /// <returns>Tuple(bool:'True if successful, else false', string:'error message')</returns>
        [OperationContract]
        ResponseValue General_Game_GameDifficultyAndLength(string difficulty, string length);
    }
}
