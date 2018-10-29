using System;

namespace kf2server_tbot_client.Utils {


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
