using kf2server_tbot_client.Browsers;
using kf2server_tbot_client.Security;
using kf2server_tbot_client.Utils;
using System;
using LogEngine;

namespace kf2server_tbot_client {

    class Program {

        static void Main(string[] args) {

            /// Initialize logger
            Logger.Logfile = Properties.Settings.Default.Logfile;
            Logger.Instance.HelpText();

            /// Initialize AuthManager
            AuthManager.ChatId = Properties.Settings.Default.ChatId;

            /// Implementing handler for ProcessExit
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ClientClose);

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

                try {
                    SeleniumManager.Quit();
                    WCFServiceManager.Quit();
                }
                catch (Exception) { }

                return;
            }


            /* TESTS BEGIN */

            //ServerAdmin.ChatConsole.SendMessage("Annual Diagnostics");
            //Console.WriteLine("Action Start");
            //Console.WriteLine(ServerAdmin.PageManager.Instance.ChangeMap.ChangeMapOnly("KF-BioticsLab").Item1);
            //Console.WriteLine("Action End");
            //sm.Quit();

            Console.ReadKey();
        }


        /// <summary>
        /// Terminates console application.
        /// <para>First, active Users in AuthManager are serialized, encrypted, then flushed to disk.</para>
        /// <para>Second, SeleniumManager is disposed, and with it, all open browsers for ServerAdmin pages.</para>
        /// <para>Third, WCFServiceManager is halted, closing all open ServiceHosts.</para>
        /// </summary>
        /// <param name="sender">Console</param>
        /// <param name="e">Close</param>
        static void ClientClose(object sender, EventArgs e) {

            Crypto.EncryptalizeUsers(AuthManager.Users);

            WCFServiceManager.Quit();

            Logger.Log(Status.GENERIC_WARNING, "Quitting Application...");

            System.Threading.Thread.Sleep(1000);

            SeleniumManager.Quit();

            Environment.Exit(0);
        }

    }
}
