using System;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;

namespace tbot_client {
    class Program {
        static void Main(string[] args) {

            System.Threading.Thread.Sleep(3000);

            bool IsStarted = false;
            while (!IsStarted) {
                try {

                    AddUsers();

                    MiscellaneousServiceAuthTest("uuid02");

                    Console.WriteLine();
                    IsStarted = true;
                } catch (System.ServiceModel.EndpointNotFoundException) { }
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


            /// Create SOAP xml
            /*
            XNamespace ns = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace myns = "http://tempuri.org/IMiscellaneousService";

            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace xsd = "http://www.w3.org/2001/XMLSchema";

            XDocument soapRequest = new XDocument(
                new XDeclaration("1.0", "UTF-8", "no"),
                new XElement(ns + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                    new XAttribute(XNamespace.Xmlns + "soap", ns),
                    new XElement(ns + "Body",
                        new XElement(myns + "Test",
                            new XElement(myns + "client",
                                new XElement(myns + "Username", "admin"),
                                new XElement(myns + "Password", "WelcomeToBrampton69")),
                            new XElement(myns + "TelegramID", telegramID)
                        )
                    )
                ));
            */


            /// Create SOAP xml
            XNamespace ns = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace myns = "http://tempuri.org";

            XNamespace xmlnsAddressing = "http://schemas.microsoft.com/ws/2005/05/addressing/none";
            string serviceAddr = "http://localhost:8733/Design_Time_Addresses/kf2server_tbot_client.Service/MiscellaneousService/";
            string serviceAction = "http://tempuri.org/IMiscellaneousService/Test";

            XDocument soapRequest = new XDocument(
                new XDeclaration("1.0", "UTF-8", "no"),
                new XElement(ns + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "s", ns),
                    new XElement(ns + "Header",
                        new XElement(xmlnsAddressing + "To",
                            new XAttribute(ns + "mustUnderstand", "1"),
                            //new XAttribute(myns + "", xmlnsAddressing),
                            serviceAddr
                        ),
                        new XElement(xmlnsAddressing + "Action",
                            new XAttribute(ns + "mustUnderstand", "1"),
                            //new XAttribute(myns + "", xmlnsAddressing),
                            serviceAction
                        )
                    ),
                    new XElement(ns + "Body",
                        new XElement(myns + "Test",
                            new XElement(myns + "client",
                                new XElement(myns + "Username", "admin"),
                                new XElement(myns + "Password", "WelcomeToBrampton69")),
                            new XElement(myns + "TelegramID", telegramID)
                        )
                    )
                ));

            Console.WriteLine(soapRequest.ToString());


            /// Requesting part 2
            
            var WebminCredentials = new NetworkCredential("admin", "WelcomeToBrampton69");
            var Handler = new HttpClientHandler { Credentials = WebminCredentials };

            using (var client = new HttpClient(Handler)) {
                client.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/IMiscellaneousService/Test");
                var content = new StringContent(soapRequest.ToString(), System.Text.Encoding.UTF8, "text/xml");
                using (var response = client.PostAsync("http://localhost:8733/Design_Time_Addresses/kf2server_tbot_client.Service/MiscellaneousService/Test", content)) {

                    System.Threading.Tasks.Task<System.IO.Stream> streamTask = response.Result.Content.ReadAsStreamAsync();
                    System.IO.Stream stream = streamTask.Result;
                    var sr = new System.IO.StreamReader(stream);
                    var soapResponse = XDocument.Load(sr);
                    Console.WriteLine(soapResponse);


                }
            }
            



            /// Requestiong?
            /*
            try
            {

                var WebminCredentials = new NetworkCredential("admin", "WelcomeToBrampton69");
                var Handler = new HttpClientHandler { Credentials = WebminCredentials };

                using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip, Credentials = WebminCredentials }) { Timeout = TimeSpan.FromHours(1) }) {
                    var request = new HttpRequestMessage() {
                        RequestUri = new Uri(@"http://localhost:8733/Design_Time_Addresses/kf2server_tbot_client.Service/MiscellaneousService/Test"),
                        Method = HttpMethod.Post
                    };

                    request.Content = new StringContent(soapRequest.ToString(), System.Text.Encoding.UTF8, "text/xml");

                    request.Headers.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
                    request.Headers.Add("SOAPAction", "http://tempuri.org/IMiscellaneousService/Test");

                    HttpResponseMessage response = client.SendAsync(request).Result;

                    if (!response.IsSuccessStatusCode) {
                        throw new Exception();
                    }

                    System.Threading.Tasks.Task<System.IO.Stream> streamTask = response.Content.ReadAsStreamAsync();
                    System.IO.Stream stream = streamTask.Result;
                    var sr = new System.IO.StreamReader(stream);
                    var soapResponse = XDocument.Load(sr);
                    Console.WriteLine(soapResponse);
                }
            }
            catch (AggregateException ex) {
                if (ex.InnerException is System.Threading.Tasks.TaskCanceledException) {
                    throw ex.InnerException;
                } else {
                    throw ex;
                }
            } catch (Exception ex) {
                throw ex;
            }
            */



            /*
            var WebminCredentials = new NetworkCredential("admin", "WelcomeToBrampton69");
            var Handler = new HttpClientHandler { Credentials = WebminCredentials };

            using (HttpClient client = new HttpClient(Handler)) {
                HttpResponseMessage wcfResponse = client.GetAsync("http://localhost:8733/Design_Time_Addresses/kf2server_tbot_client.Service/MiscellaneousService/Test").Result;
                HttpContent stream = wcfResponse.Content;
                var data = stream.ReadAsStringAsync();
                Console.WriteLine(data.Result);
            }
            */
        }


        private static void MiscellaneousServiceTest() {

            Miscellaneous.MiscellaneousServiceClient misc = new Miscellaneous.MiscellaneousServiceClient();

            misc.ClientCredentials.UserName.UserName = "admin";
            misc.ClientCredentials.UserName.Password = "WelcomeToBrampton69";

            misc.Open();

            misc.AddUser("123telegramid456", new string[] {
                "Miscellaneous.Test",
                "Miscellaneous.Pause"
            });
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
