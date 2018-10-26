using kf2server_tbot_client.Browsers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kf2server_tbot_client {

    class Program {

        static void Main(string[] args) {

            SeleniumManager sm = new SeleniumManager();

            foreach(KeyValuePair<ServerAdmin.PageType, string> wp in ServerAdmin.PageManager.Pages) {
                Console.WriteLine(wp.Key + "|" + wp.Value);
            }


            //ServerAdmin.ChatConsole.SendMessage("Annual Diagnostics");
            Console.WriteLine("Action Start");
            Console.WriteLine(ServerAdmin.PageManager.Instance.ChangeMap.ChangeMapOnly("KF-BioticsLab").Item1);
            Console.WriteLine("Action End");
            //sm.Quit();

            Console.ReadLine();
        }

    }
}
