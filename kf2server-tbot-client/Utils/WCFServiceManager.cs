using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;
using kf2server_tbot_client.Auth;
using kf2server_tbot_client.Service;

namespace kf2server_tbot_client.Utils {
    class WCFServiceManager {

        public WCFServiceManager() {

            // Retrieve Users data, provide to AuthManager
            AuthManager.Users = Crypto.DecryptalizeUsers();

            StartServices();

            //UserXMLSerialization(AuthManager.Users);

            //UserXMLDeserialization();

            Encryptalize();
            //Decryptalize();

            //UserXMLReader();

        }


        private static void StartServices() {
            ServiceHost CurrentGameServiceHost;
            ServiceHost AccessPolicyServiceHost;
            ServiceHost SettingsServiceHost;
            ServiceHost MiscellaneousServiceHost;

            CurrentGameServiceHost = new ServiceHost(typeof(CurrentGameService));
            CurrentGameServiceHost.Open();
            Console.WriteLine("{0} hosted at {1}", CurrentGameServiceHost.Description.Name,
                CurrentGameServiceHost.Description.Endpoints[0].Address);

            AccessPolicyServiceHost = new ServiceHost(typeof(AccessPolicyService));
            AccessPolicyServiceHost.Open();
            Console.WriteLine("{0} hosted at {1}", AccessPolicyServiceHost.Description.Name,
                AccessPolicyServiceHost.Description.Endpoints[0].Address);

            SettingsServiceHost = new ServiceHost(typeof(SettingsService));
            SettingsServiceHost.Open();
            Console.WriteLine("{0} hosted at {1}", SettingsServiceHost.Description.Name,
                SettingsServiceHost.Description.Endpoints[0].Address);

            MiscellaneousServiceHost = new ServiceHost(typeof(MiscellaneousService));
            MiscellaneousServiceHost.Open();
            Console.WriteLine("{0} hosted at {1}", MiscellaneousServiceHost.Description.Name,
                MiscellaneousServiceHost.Description.Endpoints[0].Address);
        }


        private static void Encryptalize() {

            #region Users XML Prep
            Users users = new Users();

            List<string> tmpRoleIDs = new List<string>();

            for (int i = 0; i < 5; i++) {

                tmpRoleIDs.Clear();
                for (int j = 0; j < i; j++)
                    tmpRoleIDs.Add("role" + j);

                Auth.Role tmpRole = new Auth.Role();
                tmpRole.RoleID = tmpRoleIDs.ToArray<string>();


                users.Accounts.Add(new Auth.Account() {
                    Username = "username" + i,
                    Password = "password" + i,
                    Roles = tmpRole
                });
            }
            #endregion

            Crypto.EncryptalizeUsers(users);
        }

        private static void Decryptalize() {

            Users users = Crypto.DecryptalizeUsers();

        }


        private static void UserXMLSerialization() {


            XmlSerializer serializer = new XmlSerializer(typeof(Users));

            TextWriter writer = new StreamWriter(Properties.Settings.Default.UsersRelFilePath);

            Users users = new Users();

            List<string> tmpRoleIDs = new List<string>();

            for (int i = 0; i < 5; i++) {

                tmpRoleIDs.Clear();
                for (int j = 0; j < i; j++)
                    tmpRoleIDs.Add("role" + j);

                Auth.Role tmpRole = new Auth.Role();
                tmpRole.RoleID = tmpRoleIDs.ToArray<string>();


                users.Accounts.Add(new Auth.Account() {
                    Username = "username" + i,
                    Password = "password" + i,
                    Roles = tmpRole
                });
            }

            serializer.Serialize(writer, users);
            writer.Close();

            Console.WriteLine("Done Serializing");
        }

        private static void UserXMLSerialization(Users users) {

            XmlSerializer serializer = new XmlSerializer(typeof(Users));

            TextWriter writer = new StreamWriter("Users.xml");

            serializer.Serialize(writer, AuthManager.Users);
            writer.Close();

            Console.WriteLine("Done Serializing");

        }


        private static void UserXMLReader() {

            using (XmlReader r = XmlReader.Create(Properties.Settings.Default.UsersRelFilePath)) {

                Dictionary<string, string> NodeAttributes = new Dictionary<string, string>();

                while (r.Read()) {

                    switch (r.NodeType) {
                        case XmlNodeType.Attribute:
                            //if(!string.IsNullOrEmpty(r.GetAttribute("nature")))
                            //    Console.WriteLine("Attr: {0}", r.GetAttribute("nature"));

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
