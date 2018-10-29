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

            // Init Browsers (Selenium)
            SeleniumManager sm = new SeleniumManager();

            // Init WCF
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ClientClose);
            WCFServiceManager wcf = new WCFServiceManager();


            foreach (KeyValuePair<ServerAdmin.PageType, string> wp in ServerAdmin.PageManager.Pages) {
                Console.WriteLine(wp.Key + "|" + wp.Value);
            }


            /* TESTS BEGIN */

            //ServerAdmin.ChatConsole.SendMessage("Annual Diagnostics");
            Console.WriteLine("Action Start");
            //Console.WriteLine(ServerAdmin.PageManager.Instance.ChangeMap.ChangeMapOnly("KF-BioticsLab").Item1);
            Console.WriteLine("Action End");
            //sm.Quit();

            Console.ReadLine();
        }


        static void ClientClose(object sender, EventArgs e) {

            Auth.Crypto.EncryptalizeUsers(Auth.AuthManager.Users);

            Console.WriteLine("Quitting Application...");

            System.Threading.Thread.Sleep(1000);

        }

    }
}
