using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;
using kf2server_tbot_client.Security;
using kf2server_tbot_client.Service;
using LogEngine;
using System.Diagnostics;

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

            if(!string.IsNullOrWhiteSpace(Properties.Settings.Default.ServiceHostURI) && 
                !string.IsNullOrWhiteSpace(Properties.Settings.Default.ServiceHostUser)) {

                AddServiceHostURIToURLACL();
            }

            KF2Service.Open();

            Logger.Log(Status.SERVICE_SUCCESS, string.Format("{0} hosted at {1}", KF2Service.Description.Name,
                KF2Service.Description.Endpoints[0].Address));

        }


        /// <summary>
        /// Adds (does not replace) a new endpoint for accessing the KF2Service based on ServiceHostURI identified in settings
        /// In addition to this URI, a user/group is required to assign the address to - hence, must also have ServiceHostUser
        /// </summary>
        /// <returns></returns>
        private static Tuple<bool, string> AddServiceHostURIToURLACL() {

            Tuple<bool, string> Result = new Tuple<bool, string>(true, "Created endpoint for ServiceHostURI: " +
                Properties.Settings.Default.ServiceHostURI);

            try {

                /// Prepares argument string for 'netsh' command
                /// First parameter is url, which is in the following form:
                /// [http / https]://[domain / +]:[port]/
                /// Where:
                ///  [http / https] is the protocol
                ///  [domain / +] is either a domain, or localhost (+)
                ///  [port] is any valid port that is not in use
                /// Example URL: http://+:8888/
                /// Second parameter is User (default: \\Users group)
                string parameter = string.Format("http add urlacl url={0} user={1}",
                    Properties.Settings.Default.ServiceHostURI, Properties.Settings.Default.ServiceHostUser);

                ProcessStartInfo psi = new ProcessStartInfo("netsh", parameter);
                Process netshProcess = new Process();

                /// Prepare various console options
                psi.Verb = "runas";
                psi.RedirectStandardError = false;
                psi.RedirectStandardOutput = false;
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;


                netshProcess.EnableRaisingEvents = true;
                netshProcess.OutputDataReceived += ((e, e2) => {

                    Console.WriteLine(e);
                    Console.WriteLine(e2);

                });

                netshProcess = Process.Start(psi);

                /// Wait until netsh cmd completed
                while (!netshProcess.HasExited) { }

                /// If netsh command failed
                if(netshProcess.ExitCode != 0) {
                    throw new SystemException(
                        "netsh failed either due to lack of elevation (run as Administrator), or because URL reservation already exists, or some other generic failure");
                }

            } catch(Exception e) {
                Result = new Tuple<bool, string>(false, string.Format("Could not create endpoint with ServiceHostURI ({0}). Error: {1}",
                    Properties.Settings.Default.ServiceHostURI, e.Message));
            }


            Logger.Log((Result.Item1) ? Status.SERVICE_SUCCESS : Status.SERVICE_WARNING, Result.Item2);

            return Result;
        }


        /// <summary>
        /// Terminates all open ServiceHosts
        /// </summary>
        public static void Quit() {

            try {

                KF2Service.Close();

            } catch (Exception) { }

        }

    }
}
