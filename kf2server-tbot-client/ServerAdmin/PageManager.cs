using kf2server_tbot_client.Browsers;
using kf2server_tbot_client.ServerAdmin.AccessPolicy;
using kf2server_tbot_client.ServerAdmin.CurrentGame;
using kf2server_tbot_client.ServerAdmin.Settings;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

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


        public Tuple<bool, string> Init(IWebDriver driver) {

            CancellationTokenSource cts = new CancellationTokenSource();
            Tuple<bool, string> result = new Tuple<bool, string>(false, "Generic Error");

            System.Threading.Tasks.Task<object> t = new System.Threading.Tasks.Task<object>(() => {
                string s = "https://google.com/";

                int a = new Random().Next();

                for(int i = 0; i < 100; i++) {

                    // Experiment with long-running tasks and termination
                    Console.WriteLine("Tinkering {0} ", a);
                    Thread.Sleep(100);
                }

                return s;
            });

            ActionFactory.Instance.EnhancedAdd(t, cts);

            try {
                if (!t.Wait(1000)) {
                    cts.Cancel();
                    cts.Dispose();
                    
                } else {
                    result =  new Tuple<bool, string>(true, t.Result.ToString());
                }
            } catch (Exception) { }

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
