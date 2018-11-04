using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tbot_client {
    class Program {
        static void Main(string[] args) {

            //MiscellaneousServiceTest();

            SettingsServiceTest();

            Console.ReadKey();
        }

        private static void MiscellaneousServiceTest() {

            Miscellaneous.MiscellaneousServiceClient misc = new Miscellaneous.MiscellaneousServiceClient();

            misc.ClientCredentials.UserName.UserName = "admin";
            misc.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

            misc.Open();

            for (int i = 0; i < 10; i++) {
                misc.AdminSay("Module Test: " + (i + 1));
            }
        }

        private static void SettingsServiceTest() {
            Settings.SettingsServiceClient set = new Settings.SettingsServiceClient();

            set.ClientCredentials.UserName.UserName = "admin";
            set.ClientCredentials.UserName.Password = "WelcomeToBrampton69";


            bool IsStarted = false;
            while(!IsStarted) {
                try {
                    Console.WriteLine(set.General_Game_GameDifficultyAndLength("hard", "long").Message);
                    Console.WriteLine(set.General_Game_GameDifficultyAndLength("hard", "normal").Message);
                    //Console.WriteLine(set.General_Game_GameLength("short").Message);
                    IsStarted = true;
                } catch(System.ServiceModel.EndpointNotFoundException) { }
                System.Threading.Thread.SpinWait(1000000);
            }
            


        }
    }
}
