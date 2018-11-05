using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace kf2server_tbot_client.Utils {

    /// <summary>
    /// Collection of utility methods used by services
    /// </summary>
    public class ServiceTools {

        /// <summary>
        /// Retrieves IP address of requester
        /// </summary>
        /// <returns>IP address of client which message was sent from</returns>
        public virtual string GetIP() {

            MessageProperties prop = OperationContext.Current.IncomingMessageProperties;

            RemoteEndpointMessageProperty endpoint =
                   prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            string ip = endpoint.Address;

            return ip;
        }

    }



    /// <summary>
    /// Custom attribute for designating a Role ID to particular service method.
    /// <para>Role IDs are in the following format: [ServiceType].[MethodName] </para>
    /// <para>E.g. for Test method in Miscellaneous service: "Miscellaneous.Type"</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    class ServiceMethodRoleIDAttribute : Attribute {

        private string id;

        public virtual string ID {
            get {
                return id;
            }
        }

        public ServiceMethodRoleIDAttribute(string _id) {
            id = _id;
        }

    }
}
