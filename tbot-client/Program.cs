using System;
using System.Collections.Generic;
using System.ServiceModel;
using tbot_client.KF2ServiceReference;

namespace tbot_client {

    class Program {

        #region Properties and Fields

        static Bot bot { get; set; }

        #endregion


        static void Main(string[] args) {

            bot = new Bot(Properties.Settings.Default.Token);

        }



        private static void ServiceTester() {
            //System.Threading.Thread.Sleep(3000);

            //bool IsStarted = false;
            //while (!IsStarted) {
            //    try {

            //        // AddUsers();

            //        //MiscellaneousServiceAuthTest("uuid02");

            //        PlayersTest();


            //        Console.WriteLine();
            //        IsStarted = true;
            //    } catch (EndpointNotFoundException) { }
            //    System.Threading.Thread.SpinWait(1000000);
            //}

            ////SettingsServiceTest();
        }

        private static void PlayersTest() {

            KF2ServiceReference.KF2ServiceClient kf2s = new KF2ServiceReference.KF2ServiceClient();

            kf2s.ClientCredentials.UserName.UserName = "admin";
            kf2s.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

            kf2s.Open();

            ResponseValue rv = kf2s.Status();
            Console.WriteLine(kf2s.Online().Data["online"]);
            rv = kf2s.Kick("lygais");

            foreach (KeyValuePair<string, string> kvp in rv.Data) {
                Console.WriteLine("{0}, {1}", kvp.Key, kvp.Value);
            }

        }


        private static void AddUsers() {
            KF2ServiceReference.KF2ServiceClient kf2s = new KF2ServiceReference.KF2ServiceClient();

            kf2s.ClientCredentials.UserName.UserName = "admin";
            kf2s.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

            kf2s.Open();

            kf2s.AddUser("uuid01", new string[] {
                "Miscellaneous.Pause"
            });

            kf2s.AddUser("uuid02", new string[] {
                "Miscellaneous.Pause",
                "Miscellaneous.Test"
            });

            kf2s.Close();
        }


        private static void MiscellaneousServiceAuthTest(string telegramID) {

            //using (Miscellaneous.MiscellaneousServiceClient misc = new Miscellaneous.MiscellaneousServiceClient()) {

            KF2ServiceReference.KF2ServiceClient kf2s = new KF2ServiceReference.KF2ServiceClient();

            kf2s.ClientCredentials.UserName.UserName = "admin";
                kf2s.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

                kf2s.Open();

                using (OperationContextScope scope = new OperationContextScope(kf2s.InnerChannel)) {
                    MessageHeader<string> header = new MessageHeader<string>("uuid02");
                    var untyped = header.GetUntypedHeader("TelegramID", "");
                    OperationContext.Current.OutgoingMessageHeaders.Add(untyped);

                    kf2s.Test();
                }
            //}
        }


        private static void SettingsServiceTest() {
            KF2ServiceReference.KF2ServiceClient kf2s = new KF2ServiceReference.KF2ServiceClient();

            kf2s.ClientCredentials.UserName.UserName = "admin";
            kf2s.ClientCredentials.UserName.Password = "WelcomeToBrampton69";


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
