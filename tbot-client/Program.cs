using LogEngine;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace tbot_client {

    /// <summary>
    /// Launcher class
    /// </summary>
    class Program {

        #region Properties and Fields

        static Bot bot { get; set; }

        #endregion

        /// <summary>
        /// Capture console window close (exit) event
        /// </summary>
        /// <param name="Handler"><see cref="WindowClose"/></param>
        /// <param name="Add"></param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine();


        static void Main(string[] args) {

            /// Assigning handler for console window exit
            SetConsoleCtrlHandler(new HandlerRoutine(WindowClose), true);

            /// Initialize logger
            Logger.Logfile = Properties.Settings.Default.Logfile;
            Logger.Instance.HelpText();

            /// Start bot with token from settings file
            try {
                bot = new Bot(Properties.Settings.Default.Token);
            } catch (Exception e) {
                Logger.Log(Status.TELEGRAM_FAILURE, 
                    string.Format("Failed to start telegram bot (error: {0})", e.Message));
            }

            while (true) {
                Console.ReadKey();
            }
        }



        /// <summary>
        /// Terminates console application.
        /// <para>First, closing message sent out to active telegram chat.</para>
        /// <para>Second, closing message logged to logfile, and logger instance is disposed.</para>
        /// </summary>
        /// <returns></returns>
        static bool WindowClose() {

            try {
                bot.SendTextMessageAsync(
                    chatId: bot.Chat.Id,
                    text: "I don't feel so good..."
                );

                Logger.Log(Status.GENERIC_WARNING, "Quitting Application...");
                Logger.Instance.Dispose();
            }
            catch (Exception) { }

            return true;

        }
    }
}
