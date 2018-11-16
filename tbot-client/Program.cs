using System;
using System.Collections.Generic;
using System.ServiceModel;
using tbot_client.Miscellaneous;

namespace tbot_client {
    class Program {
        static void Main(string[] args) {

            System.Threading.Thread.Sleep(3000);

            bool IsStarted = false;
            while (!IsStarted) {
                try {

                    // AddUsers();

                    //MiscellaneousServiceAuthTest("uuid02");

                    MiscellaneousServiceTest();


                    Console.WriteLine();
                    IsStarted = true;
                } catch (EndpointNotFoundException) { }
                System.Threading.Thread.SpinWait(1000000);
            }

            //SettingsServiceTest();

            Console.ReadKey();
        }


        private static void AddUsers() {
            Miscellaneous.MiscellaneousServiceClient misc = new Miscellaneous.MiscellaneousServiceClient();

            misc.ClientCredentials.UserName.UserName = "admin";
            misc.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

            misc.Open();

            misc.AddUser("uuid01", new string[] {
                "Miscellaneous.Pause"
            });

            misc.AddUser("uuid02", new string[] {
                "Miscellaneous.Pause",
                "Miscellaneous.Test"
            });

            misc.Close();
        }


        private static void MiscellaneousServiceAuthTest(string telegramID) {

            //using (Miscellaneous.MiscellaneousServiceClient misc = new Miscellaneous.MiscellaneousServiceClient()) {

                Miscellaneous.MiscellaneousServiceClient misc = new Miscellaneous.MiscellaneousServiceClient();

                misc.ClientCredentials.UserName.UserName = "admin";
                misc.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

                misc.Open();

                using (OperationContextScope scope = new OperationContextScope(misc.InnerChannel)) {
                    MessageHeader<string> header = new MessageHeader<string>("uuid02");
                    var untyped = header.GetUntypedHeader("TelegramID", "");
                    OperationContext.Current.OutgoingMessageHeaders.Add(untyped);

                    misc.Test();
                }
            //}
        }


        private static void MiscellaneousServiceTest() {

            Miscellaneous.MiscellaneousServiceClient misc = new Miscellaneous.MiscellaneousServiceClient();

            misc.ClientCredentials.UserName.UserName = "admin";
            misc.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

            misc.Open();

            ResponseValue rv = misc.Status();

            foreach(KeyValuePair<string, string> kvp in rv.Data) {
                Console.WriteLine("{0}, {1}", kvp.Key, kvp.Value);
            }
        }

        private static void SettingsServiceTest() {
            Settings.SettingsServiceClient set = new Settings.SettingsServiceClient();

            set.ClientCredentials.UserName.UserName = "admin";
            set.ClientCredentials.UserName.Password = "WelcomeToBrampton69";


            bool IsStarted = false;
            while(!IsStarted) {
                try {
                    //Console.WriteLine(set.General_Game_GameDifficultyAndLength("hard", "long").Message);
                    //Console.WriteLine(set.General_Game_GameDifficultyAndLength("hard", "normal").Message);
                    //Console.WriteLine(set.General_Game_GameLength("short").Message);
                    Console.WriteLine();
                    IsStarted = true;
                } catch(System.ServiceModel.EndpointNotFoundException) { }
                System.Threading.Thread.SpinWait(1000000);
            }
            


        }
    }
}
