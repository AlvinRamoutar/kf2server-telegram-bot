using kf2server_tbot_client.Browsers;
using kf2server_tbot_client.Security;
using kf2server_tbot_client.Utils;
using System;
using LogEngine;
using System.Runtime.InteropServices;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in command-based controls for Killing Floor 2 (TripWire)
/// Alvin Ramoutar, 2018
/// </summary>
namespace kf2server_tbot_client {

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
            WCFServiceManager wcf = null;

            try {

                /// Performs assignment of decrypted Users.dll contents to Users property of AuthManager
                AuthManager.Users = Crypto.DecryptalizeUsers();

                /// Init Browsers (Selenium)
                //sm = new SeleniumManager();

                /// Init WCF Service
                wcf = new WCFServiceManager();

                /// Assigns ChatId to AuthManager (if it exists in Settings [has been bound in the past])
                if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ChatId)) {
                    Logger.Log(Status.SERVICE_WARNING, "There is no Telegram chat bound to this server.");
                } else {
                    AuthManager.ChatId = Properties.Settings.Default.ChatId;
                }

            }
            catch (Exception e) {

                Logger.Log(Status.GENERIC_FAILURE, e.Message);

                WindowClose();
            }


            while (true) {
                Console.ReadKey();
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

                WCFServiceManager.Quit();

                Logger.Log(Status.GENERIC_WARNING, "Quitting Application...");
                Logger.Instance.Dispose();

                /// This should really close the console window itself...
                SeleniumManager.Quit();
            } catch (Exception) { }

            return true;

        }
    }
}
