using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NHtmlUnit;

namespace kf2server_telegrambot {

    class ChatMsg {
        string ajax = "1";
        string message = "Annual Diagnostics LV2; please disregard this message.";
        string teamsay = "1";
    }

    class Program {
        static void Main(string[] args) {

            TestSelentium ts = new TestSelentium();
            

            //Task<HttpResponseMessage> response = powst3();
            //Task<HttpResponseMessage> response = powst2(new ChatMsg());
            //response.Wait();

            /*
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(new {
                ajax = "1",
                message = "Annual Diagnostics LV2; please disregard this message.",
                teamsay = "1"
            }));
            */

            //Console.WriteLine(response.Result);
             //Console.WriteLine(response.Result.StatusCode);
            

            Console.ReadLine();
        }

        private static async Task<HttpResponseMessage> powst() {
            HttpClient hc = new HttpClient();

            var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("ajax", "1"),
                new KeyValuePair<string, string>("message", "Annual Diagnostics LV2; please disregard this message."),
                new KeyValuePair<string, string>("teamsay", "1")
            });

            return await hc.PostAsync("http://kf2server.rhome.net:8080/ServerAdmin/current/chat+frame+data", content);
        }

        private static async Task<HttpResponseMessage> powst2(ChatMsg chatmsg) {
            HttpClient hc = new HttpClient();


            //return await hc.PostAsJsonAsync("http://kf2server.rhome.net:8080/ServerAdmin/current/chat+frame+data", chatmsg);
            return await hc.PostAsJsonAsync("http://kf2server.rhome.net:8080/ServerAdmin/current/chat+frame+data",
                Newtonsoft.Json.JsonConvert.SerializeObject(new {
                    ajax = "1",
                    message = "Annual Diagnostics LV2; please disregard this message.",
                    teamsay = "1"
                }));
        }

        private static async Task<HttpResponseMessage> powst3() {
            string urlAddress = "http://kf2server.rhome.net:8080/ServerAdmin/current/info";

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(urlAddress);

            /// Post link
            doc = web.Load("http://kf2server.rhome.net:8080/ServerAdmin/current/chat+frame+data");

            /// get the form
            var form = doc.DocumentNode.SelectSingleNode("//form[@id='chatform']");

            /// get the form URI
            string actionValue = form.Attributes["action"]?.Value;
            System.Uri uri = new System.Uri(actionValue);

            /// Populate the form variable
            var formVariables = new List<KeyValuePair<string, string>>();
            formVariables.Add(new KeyValuePair<string, string>("chatmessage", "Annual LV2 Diagnostics - Success"));
            var formContent = new FormUrlEncodedContent(formVariables);

            /// submit the form
            HttpClient client = new HttpClient();
            return await client.PostAsync(uri, formContent);
        }
        
    }

}
