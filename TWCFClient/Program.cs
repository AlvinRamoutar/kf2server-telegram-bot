using System;

namespace TWCFClient {
    class Program {
        static void Main(string[] args) {

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ClientClose);

            


            WCFServiceManager wcf = new WCFServiceManager();

            Console.ReadKey();
        }


        static void ClientClose(object sender, EventArgs e) {

            Auth.Crypto.EncryptalizeUsers(Auth.AuthManager.Users);

            Console.WriteLine("Quitting Application...");

            System.Threading.Thread.Sleep(1000);

        }
    }
}
