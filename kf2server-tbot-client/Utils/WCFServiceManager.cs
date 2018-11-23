﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;
using kf2server_tbot_client.Security;
using kf2server_tbot_client.Service;
using LogEngine;

namespace kf2server_tbot_client.Utils {

    /// <summary>
    /// Handles WCF-related operations, notably ServiceHosts
    /// </summary>
    class WCFServiceManager {

        #region Properties and Fields
        private static ServiceHost KF2Service;
        #endregion

        /// <summary>
        /// Calls StartServices for WCF servicehosts
        /// </summary>
        public WCFServiceManager() {

            StartServices();

        }


        /// <summary>
        /// Begin starting service host for KF2Service
        /// </summary>
        private static void StartServices() {

            KF2Service = new ServiceHost(typeof(KF2Service));

            if(!string.IsNullOrWhiteSpace(Properties.Settings.Default.ServiceHostURI)) {

                var currentEndpoint = KF2Service.Description.Endpoints.First(e => e.Contract.ContractType == typeof(KF2Service));
                currentEndpoint.Address = new EndpointAddress(new Uri(Properties.Settings.Default.ServiceHostURI), 
                    currentEndpoint.Address.Identity, currentEndpoint.Address.Headers);

            }

            KF2Service.Open();

            Logger.Log(Status.SERVICE_SUCCESS, string.Format("{0} hosted at {1}", KF2Service.Description.Name,
                KF2Service.Description.Endpoints[0].Address));

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

                KF2Service.Close();

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
