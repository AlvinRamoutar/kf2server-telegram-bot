using System;
using System.Collections.Generic;
using System.ServiceModel;
using tbot_client.CurrentGame;
using tbot_client.Miscellaneous;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace tbot_client {

    class Program {

        static ITelegramBotClient botClient;

        static void Main(string[] args) {

            botClient = new TelegramBotClient("700284658:AAGYopYqRltTjLUw-V9V4PKAj-VYYP0T5fY");

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            botClient.GetFi
            System.Threading.Thread.Sleep(int.MaxValue);

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

            Console.ReadKey();
        }


        static async void Bot_OnMessage(object sender, MessageEventArgs e) {
            if (e.Message.Text != null) {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                await botClient.SendTextMessageAsync(
                  chatId: e.Message.Chat,
                  text: "You said:\n" + e.Message.Text
                );
            }

            if(e.Message.) {

            }
        }


        private static void PlayersTest() {

            CurrentGameServiceClient cgsc = new CurrentGameServiceClient();

            cgsc.ClientCredentials.UserName.UserName = "admin";
            cgsc.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

            cgsc.Open();

            CurrentGame.ResponseValue rv = cgsc.Status();
            Console.WriteLine(cgsc.Online().Data["online"]);
            rv = cgsc.Kick("lygais");

            foreach (KeyValuePair<string, string> kvp in rv.Data) {
                Console.WriteLine("{0}, {1}", kvp.Key, kvp.Value);
            }

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
