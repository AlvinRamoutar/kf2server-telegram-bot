using System;
using System.IO;
using System.Security.AccessControl;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace LogEngine {

    /// <summary>
    /// All possible operation statuses
    /// </summary>
    public enum Status {
        NONE,

        GENERIC_SUCCESS,
        GENERIC_INFO,
        GENERIC_WARNING,
        GENERIC_FAILURE,

        PAGELOAD_SUCCESS,
        PAGELOAD_INFO,
        PAGELOAD_WARNING,
        PAGELOAD_FAILURE,

        SERVERADMIN_SUCCESS,
        SERVERADMIN_INFO,
        SERVERADMIN_WARNING,
        SERVERADMIN_FAILURE,

        TELEGRAM_SUCCESS,
        TELEGRAM_INFO,
        TELEGRAM_WARNING,
        TELEGRAM_FAILURE,

        LOGGER_WARNING,
        LOGGER_ERROR
    }


    /// <summary>
    /// Handles data logging to console and file
    /// </summary>
    public class Logger {


        #region Singleton Structure
        private static Logger instance = null;
        private static readonly object padlock = new object();
        public static string Logfile = null;

        public static Logger Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new Logger();

                        /// Logs to file depending on if Logger object has read/write access
                        ///  to @Properties.Settings.Default.Logfile
                        try {
                            DirectorySecurity ds = Directory.GetAccessControl(
                                Path.Combine(Directory.GetCurrentDirectory(), Logfile));
                        }
                        catch(DirectoryNotFoundException) {  } /// Means file doesn't exist. That's fine; it'll be made.
                        catch (Exception e) {
                            Log(Status.LOGGER_ERROR, "Executing user can not write log data to " +
                                Logfile + ": " + e.ToString());
                        }
                    }
                    return instance;
                }
            }
        }

        Logger() { }
        #endregion


        #region Properties and Fields
        private static object Locker = new object();
        #endregion


        /// <summary>
        /// Handles console and logfile output to @Properties.Settings.Default.Logfile
        /// </summary>
        /// <param name="status">Status prefix for entry, such as 'ERROR', 'WARNING', etc.</param>
        /// <param name="msg">Actual message body of the entry</param>
        /// <param name="isCustom">Whether or not to create a custom-formatted, console-only entry</param>
        public static void Log(Status status, String msg, bool isCustom = false) {

            // Perform quick init if not already
            if (Logger.Instance == null) {
                object tmp = Logger.Instance;
                tmp = null;
            }

            String statusText = (status == Status.NONE) ? "" : status.ToString();
            String logLine = msg;

            if (!isCustom) {
                logLine = "[" + DateTime.Now + "]" + "[" + statusText + "] " + msg;
            }

            try {
                if (!isCustom) {

                    /// Using lock to ensure sequential writing to log file, which prevents StreamWriter file locks
                    lock (Locker) {

                        StreamWriter file = new StreamWriter(Logfile, true);

                        file.WriteLine(logLine);
                        file.Flush();
                        file.Close();
                    }
                }


                /// Configure foreground color depending on status category
                string statusCategoryText = (status.ToString().Contains("_")) ?
                    status.ToString().Substring(status.ToString().LastIndexOf("_") + 1) : status.ToString();

                switch (statusCategoryText) {
                    case "SUCCESS":
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case "INFO":
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case "WARNING":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case "FAILURE":
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }

                Console.WriteLine(logLine);
                Console.ForegroundColor = ConsoleColor.White;

            }
            catch (Exception e) {

                Console.WriteLine("[" + DateTime.Now + "]" + Status.LOGGER_ERROR
                    + " Problem when handling log file: " + e.ToString());

            }
        }


        /// <summary>
        /// WIP
        /// Help text w. command descriptions
        /// </summary>
        public void HelpText() {

            Log(Status.NONE, "================================================================================", true);
            Log(Status.NONE, "= KF2 Telegram Bot", true);
            Log(Status.NONE, "= An experiment in command-based controls for Killing Floor 2 (TripWire)", true);
            Log(Status.NONE, "=-------------------------------------------------------------------------------", true);
            Log(Status.NONE, "=                                                           Alvin Ramoutar, 2018", true);
            Log(Status.NONE, "================================================================================", true);
        }


        /// <summary>
        /// Disposal of current logger instance
        /// </summary>
        public void Dispose() {
            instance = null;
        }

    }
}
