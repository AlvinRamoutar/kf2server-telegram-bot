using kf2server_tbot_client.Browsers;
using kf2server_tbot_client.ServerAdmin.AccessPolicy;
using kf2server_tbot_client.ServerAdmin.CurrentGame;
using kf2server_tbot_client.ServerAdmin.Settings;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace kf2server_tbot_client.ServerAdmin {

    public enum PageType {
        Login,
        ServerInfo, ChangeMap, Players,
        Passwords,
        General, GameTypes
    }

    class PageManager {

        #region Singleton Structure
        private static PageManager instance = null;
        private static readonly object padlock = new object();

        public static PageManager Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new PageManager();
                    }
                    return instance;
                }
            }
        }
        PageManager() { }
        #endregion

        IWebDriver Driver { get; set; }

        public Login Login { get; set; }
        public ChatConsole ChatConsole { get; set; }
        public ServerInfo ServerInfo { get; set; }
        public ChangeMap ChangeMap { get; set; }
        public Players Players { get; set; }
        public Passwords Passwords { get; set; }
        public General General { get; set; }
        public GameTypes GameTypes { get; set; }

        public static Dictionary<PageType, string> Pages = new Dictionary<PageType, string>();


        public async Task<Tuple<bool, string>> Init4(IWebDriver driver) {

            Tuple<bool, string> result = new Tuple<bool, string>(false, "Generic Error");

            Thread t = new Thread(() => {
                int a = new Random().Next();

                Console.WriteLine("ID: {0}", a);
                for (int i = 0; i < new Random().Next(20, 40); i++) {
                    // Experiment with long-running tasks and termination
                    Console.Write(".");
                    Thread.Sleep(100);
                }

                result = new Tuple<bool, string>(true, "Success in the thread!");
            });

            Task<Tuple<bool, string>> actionTask = new Task<Tuple<bool, string>>(() => {
                return ActionFactory.Instance.Add(t);
            });

            result = await actionTask;

            return result;
        }

        public Tuple<bool, string> Init(IWebDriver driver) {

            CancellationTokenSource cts = new CancellationTokenSource();
            Tuple<bool, string> result = new Tuple<bool, string>(false, "Generic Error");


            Thread t = new Thread(() => {
                int a = new Random().Next();

                for (int i = 0; i < new Random().Next(20, 40); i++) {
                    // Experiment with long-running tasks and termination
                    //Console.Write(".");
                    Thread.Sleep(100);
                }

                result = new Tuple<bool, string>(true, "Success in the thread!");
            });

            ActionFactory.Instance.EnhancedAdd(t, cts);

            new Task(() => {
                Thread.Sleep(3000);
                cts.Cancel();
                cts.Dispose();
            }).Start();

            return result;
        }

        public Tuple<bool, string> Init2(IWebDriver driver) {

            string errorMessage = null;

            Login = Login.Instance;
            Login.Driver = driver;
            Login.SignIn();

            ChatConsole = ChatConsole.Instance;
            ChatConsole.Driver = driver;

            ServerInfo = ServerInfo.Instance;
            ServerInfo.Driver = driver;
            ServerInfo.Init();

            ChangeMap = ChangeMap.Instance;
            ChangeMap.Driver = driver;
            ChangeMap.Init();

            Players = Players.Instance;
            Players.Driver = driver;
            Players.Init();

            Passwords = Passwords.Instance;
            Passwords.Driver = driver;
            Passwords.Init();

            General = General.Instance;
            General.Driver = driver;
            General.Init();

            GameTypes = GameTypes.Instance;
            GameTypes.Driver = driver;
            GameTypes.Init();

            return new Tuple<bool, string>(true, errorMessage);
        }

    }

}
