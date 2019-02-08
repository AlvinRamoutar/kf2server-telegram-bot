using System;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.Utils {

    /// <summary>
    /// Custom attribute for designating a Role ID to particular service method.
    /// <para>Role IDs are in the following format: [ServiceType].[MethodName] </para>
    /// <para>E.g. for Test method in Miscellaneous service: "Miscellaneous.Type"</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    class RoleIDAttribute : Attribute {

        private string id;

        public virtual string ID {
            get {
                return id;
            }
        }

        public RoleIDAttribute(string _id) {
            id = _id;
        }

    }
}
