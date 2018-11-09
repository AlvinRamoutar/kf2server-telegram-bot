using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;
using kf2server_tbot_client.Security;
using kf2server_tbot_client.Service;

namespace kf2server_tbot_client.Utils {

    /// <summary>
    /// Handles WCF-related operations, notably ServiceHosts
    /// </summary>
    class WCFServiceManager {

        #region Properties and Fields
        private static ServiceHost CurrentGameServiceHost;
        private static ServiceHost AccessPolicyServiceHost;
        private static ServiceHost SettingsServiceHost;
        private static ServiceHost MiscellaneousServiceHost;
        #endregion

        /// <summary>
        /// Calls StartServices for WCF servicehosts
        /// </summary>
        public WCFServiceManager() {

            StartServices();

        }


        /// <summary>
        /// Begin starting all ServiceHosts for each WCF service.
        /// <para>Each WCF service addresses a category from the KF2 ServerAdmin sidebar</para>
        /// </summary>
        private static void StartServices() {

            CurrentGameServiceHost = new ServiceHost(typeof(CurrentGameService));
            CurrentGameServiceHost.Open();

            LogEngine.Log(Status.SERVICE_SUCCESS, string.Format("{0} hosted at {1}", CurrentGameServiceHost.Description.Name,
                CurrentGameServiceHost.Description.Endpoints[0].Address));


            AccessPolicyServiceHost = new ServiceHost(typeof(AccessPolicyService));
            AccessPolicyServiceHost.Open();

            LogEngine.Log(Status.SERVICE_SUCCESS, string.Format("{0} hosted at {1}", AccessPolicyServiceHost.Description.Name,
                AccessPolicyServiceHost.Description.Endpoints[0].Address));


            SettingsServiceHost = new ServiceHost(typeof(SettingsService));
            SettingsServiceHost.Open();

            LogEngine.Log(Status.SERVICE_SUCCESS, string.Format("{0} hosted at {1}", SettingsServiceHost.Description.Name,
                SettingsServiceHost.Description.Endpoints[0].Address));

            MiscellaneousServiceHost = new ServiceHost(typeof(MiscellaneousService));
            MiscellaneousServiceHost.Open();

            LogEngine.Log(Status.SERVICE_SUCCESS, string.Format("{0} hosted at {1}", MiscellaneousServiceHost.Description.Name,
                MiscellaneousServiceHost.Description.Endpoints[0].Address));
        }


        [Obsolete]
        private static void UserXMLSerialization() {


            XmlSerializer serializer = new XmlSerializer(typeof(Users));

            TextWriter writer = new StreamWriter(Properties.Settings.Default.UsersRelFilePath);

            Users users = new Users();

            List<string> tmpRoleIDs = new List<string>();

            for (int i = 0; i < 5; i++) {

                tmpRoleIDs.Clear();
                for (int j = 0; j < i; j++)
                    tmpRoleIDs.Add("role" + j);

                Security.Role tmpRole = new Security.Role();
                tmpRole.RoleID = tmpRoleIDs.ToArray<string>();


                users.Accounts.Add(new Security.Account() {
                    TelegramUUID = "telegramUUID" + i,
                    SteamUUID = "steamUUID" + i,
                    Roles = tmpRole
                });
            }

            serializer.Serialize(writer, users);
            writer.Close();

            Console.WriteLine("Done Serializing");
        }

        [Obsolete]
        private static void UserXMLSerialization(Users users) {

            XmlSerializer serializer = new XmlSerializer(typeof(Users));

            TextWriter writer = new StreamWriter("Users.xml");

            serializer.Serialize(writer, AuthManager.Users);
            writer.Close();

            Console.WriteLine("Done Serializing");

        }

        [Obsolete]
        private static void UserXMLReader() {

            using (XmlReader r = XmlReader.Create(Properties.Settings.Default.UsersRelFilePath)) {

                Dictionary<string, string> NodeAttributes = new Dictionary<string, string>();

                while (r.Read()) {

                    switch (r.NodeType) {
                        case XmlNodeType.Attribute:
                            //if(!string.IsNullOrEmpty(r.GetAttribute("nature")))
                            ///    Console.WriteLine("Attr: {0}", r.GetAttribute("nature"));

                            break;

                        case XmlNodeType.Element:

                            if (r.HasAttributes) {
                                int i = 0;
                                while (r.MoveToNextAttribute()) {

                                    NodeAttributes.Add(r.Name, r.Value);
                                    i++;
                                }
                                r.MoveToElement();
                            }

                            break;
                        case XmlNodeType.Text:

                            break;
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.EndElement:
                            break;
                    }


                }

                foreach (KeyValuePair<string, string> kvp in NodeAttributes) {
                    Console.WriteLine("Name: \"{0}\" Value:\"{1}\"", kvp.Key, kvp.Value);
                }
            }
        }


        /// <summary>
        /// Terminates all open ServiceHosts
        /// </summary>
        public static void Quit() {

            try {

                CurrentGameServiceHost.Close();
                AccessPolicyServiceHost.Close();
                SettingsServiceHost.Close();
                MiscellaneousServiceHost.Close();

            } catch (Exception) { }

        }




        /*
         * NEEDED for non-admin binding addr
        public void Start() {
            string everyone = new System.Security.Principal.SecurityIdentifier(
                "S-1-1-0").Translate(typeof(System.Security.Principal.NTAccount)).ToString();

            string parameter = @"http add urlacl url=http://+:8888/ user=\" + everyone;

            ProcessStartInfo psi = new ProcessStartInfo("netsh", parameter);

            psi.Verb = "runas";
            psi.RedirectStandardOutput = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }
    }*/

    }
}
