using kf2server_tbot.Browsers;
using kf2server_tbot.Security;
using kf2server_tbot.Utils;
using System;
using LogEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot {

    /// <summary>
    /// Launcher class
    /// </summary>
    class Program {


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

            /// Initialize AuthManager
            AuthManager.ChatId = Properties.Settings.Default.ChatId;

            SeleniumManager sm = null;
            Bot bot = null;

            try {

                /// Performs assignment of decrypted Users.dll contents to Users property of AuthManager
                AuthManager.Users = Crypto.DecryptalizeUsers();
                Logger.Log(Status.GENERIC_INFO, "Startup Task Complete (1/3) - Loading Users");

                /// Init Browsers (Selenium)
                sm = new SeleniumManager();
                Logger.Log(Status.GENERIC_INFO, "Startup Task Complete (2/3) - ServerAdmin Browsers");

                /// Init Bot
                bot = new Bot(Properties.Settings.Default.TelegramBotToken);

                /// Assigns ChatId to AuthManager (if it exists in Settings [has been bound in the past])
                if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ChatId)) {
                    Logger.Log(Status.TELEGRAM_FAILURE, "There is no Telegram chat bound to this server.");
                    Logger.Log(Status.TELEGRAM_INFO, "Send a message with the '/setup' command. Enter the provided ChatId below.");

                    bool isChatIdSupplied = false;
                    string _chatId;
                    string _key;

                    while(!isChatIdSupplied) {

                        _chatId = Console.ReadLine();

                        bot.Setup(SetupStage.HandshakeMessage, null, new List<string>() { _chatId });

                        Logger.Log(Status.TELEGRAM_INFO, "If you received the token message successfully in Telegram, then enter 'Y', otherwise enter 'N'");

                        _key = Console.ReadKey().Key.ToString();

                        if (!_key.ToUpper().Equals("Y")) {
                            Logger.Log(Status.TELEGRAM_INFO, "Please try again");
                        } else {
                            bot.Setup(SetupStage.PostSetup, null, null);
                            isChatIdSupplied = true;
                        }

                    }
                } else {

                    AuthManager.ChatId = Properties.Settings.Default.ChatId;

                }

                Logger.Log(Status.GENERIC_INFO, "Startup Task Complete (3/3) - Telegram Bot");
                Logger.Log(Status.GENERIC_SUCCESS, "Now accepting commands (and brainz)");

                Console.ReadKey();

            }
            catch (Exception e) {

                Logger.Log(Status.GENERIC_FAILURE, e.Message);

                WindowClose();
            }
        }


        /// <summary>
        /// Terminates console application.
        /// <para>First, active Users in AuthManager are serialized, encrypted, then flushed to disk.</para>
        /// <para>Second, WCFServiceManager is halted, closing all open ServiceHosts.</para>
        /// <para>Third, Close message logged to logfile, and logger instance disposed.</para>
        /// <para>Fourth, SeleniumManager is disposed, and with it, all open browsers for ServerAdmin pages.</para>
        /// </summary>
        /// <returns></returns>
        static bool WindowClose() {

            try {
                Crypto.EncryptalizeUsers(AuthManager.Users);

                Logger.Log(Status.GENERIC_WARNING, "Quitting Application...");
                Logger.Instance.Dispose();

                /// This should really close the console window itself...
                SeleniumManager.Quit();
            }
            catch (Exception) { }

            return true;

        }
    }
}
