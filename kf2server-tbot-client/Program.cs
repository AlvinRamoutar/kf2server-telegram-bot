using kf2server_tbot_client.Browsers;
using kf2server_tbot_client.Utils;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kf2server_tbot_client {

    class Program {

        static void Main(string[] args) {

            LogEngine.Instance.HelpText();

            /// Implementing handler for ProcessExit
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ClientClose);

            SeleniumManager sm = null;
            WCFServiceManager wcf = null;

            try {

                /// Init Browsers (Selenium)
                //sm = new SeleniumManager();

                /// Init WCF
                wcf = new WCFServiceManager();

            } catch(Exception e) {

                LogEngine.Log(Status.GENERIC_FAILURE, e.Message);

                try {
                    SeleniumManager.Quit();
                    WCFServiceManager.Quit();
                }
                catch (Exception) { }

                return;
            }

            
            // Testing that all windows opened via window handlers
            foreach (KeyValuePair<ServerAdmin.PageType, string> wp in ServerAdmin.PageManager.Pages) {
                Console.WriteLine(wp.Key + "|" + wp.Value);
            }


            /* TESTS BEGIN */

            //ServerAdmin.ChatConsole.SendMessage("Annual Diagnostics");
            //Console.WriteLine("Action Start");
            //Console.WriteLine(ServerAdmin.PageManager.Instance.ChangeMap.ChangeMapOnly("KF-BioticsLab").Item1);
            //Console.WriteLine("Action End");
            //sm.Quit();

            Console.ReadLine();
        }


        static void ClientClose(object sender, EventArgs e) {

            Auth.Crypto.EncryptalizeUsers(Auth.AuthManager.Users);

            LogEngine.Log(Status.GENERIC_INFO, "Quitting Application...");

            System.Threading.Thread.Sleep(1000);

        }

    }
}
