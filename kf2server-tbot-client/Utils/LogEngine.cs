using System;
using System.IO;
using System.Security.AccessControl;

namespace kf2server_tbot_client.Utils {


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

        SERVICE_SUCCESS,
        SERVICE_INFO,
        SERVICE_WARNING,
        SERVICE_FAILURE,

        LOGGER_WARNING,
        LOGGER_ERROR
    }


    /// <summary>
    /// Handles data logging to console and file
    /// </summary>
    class LogEngine {

        
        #region Singleton Structure
        private static LogEngine instance = null;
        private static readonly object padlock = new object();

        public static LogEngine Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new LogEngine();

                        /// Logs to file depending on if Logger object has read/write access
                        ///  to @Properties.Settings.Default.Logfile
                        try {
                            DirectorySecurity ds = Directory.GetAccessControl(
                                Path.Combine(Directory.GetCurrentDirectory(), Properties.Settings.Default.Logfile));
                        }
                        catch (Exception e) {
                            Log(Status.LOGGER_ERROR, "Executing user can not write log data to " +
                                Properties.Settings.Default.Logfile + ": " + e.ToString());
                        }
                    }
                    return instance;
                }
            }
        }

        LogEngine() { }
        #endregion


        #region Properties and Fields
        private static object Locker = new object();

        private StreamWriter sw { get; set; }
        #endregion


        /// <summary>
        /// Handles console and logfile output to @Properties.Settings.Default.Logfile
        /// </summary>
        /// <param name="status">Status prefix for entry, such as 'ERROR', 'WARNING', etc.</param>
        /// <param name="msg">Actual message body of the entry</param>
        /// <param name="isCustom">Whether or not to create a custom-formatted, console-only entry</param>
        public static void Log(Status status, String msg, bool isCustom = false) {

            // Perform quick init if not already
            if (LogEngine.Instance == null) {
                object tmp = LogEngine.Instance;
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

                        StreamWriter file = new StreamWriter(Properties.Settings.Default.Logfile, true);

                        file.WriteLine(logLine);
                        file.Flush();
                        file.Close();
                    }
                }


                /// Configure foreground color depending on status category
                string statusCategoryText = (status.ToString().Contains("_")) ? 
                    status.ToString().Substring(status.ToString().LastIndexOf("_") + 1) : status.ToString();
                    
                switch(statusCategoryText) {
                    case "SUCCESS":
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case "INFO":
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case "WARNING":
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
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
        /// Help text w. command descriptions
        /// </summary>
        public void HelpText() {
            Log(Status.NONE, "================================================================", true);
            Log(Status.NONE, "Project Title", true);
            Log(Status.NONE, "Project Description", true);
            Log(Status.NONE, "----------------------------------------------------------------", true);
            Log(Status.NONE, "COMMANDS", true);
            Log(Status.NONE, "  exit     |  Terminates application", true);
            Log(Status.NONE, "================================================================", true);
        }


        /// <summary>
        /// Disposal of current logger instance
        /// </summary>
        public void Dispose() {
            instance = null;
        }

    }
}