﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kf2server_tbot_client.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://kf2server.rhome.net:8080/ServerAdmin/")]
        public string KF2ServerURL {
            get {
                return ((string)(this["KF2ServerURL"]));
            }
            set {
                this["KF2ServerURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("admin")]
        public string WebAdminUsername {
            get {
                return ((string)(this["WebAdminUsername"]));
            }
            set {
                this["WebAdminUsername"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("WelcomeToBrampton69")]
        public string WebAdminPassword {
            get {
                return ((string)(this["WebAdminPassword"]));
            }
            set {
                this["WebAdminPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("current/info")]
        public string ServerInfoURL {
            get {
                return ((string)(this["ServerInfoURL"]));
            }
            set {
                this["ServerInfoURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("current/change")]
        public string ChangeMapURL {
            get {
                return ((string)(this["ChangeMapURL"]));
            }
            set {
                this["ChangeMapURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("current/players")]
        public string PlayersURL {
            get {
                return ((string)(this["PlayersURL"]));
            }
            set {
                this["PlayersURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("policy/passwords")]
        public string PasswordsURL {
            get {
                return ((string)(this["PasswordsURL"]));
            }
            set {
                this["PasswordsURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("settings/general#SG_Game")]
        public string GeneralURL {
            get {
                return ((string)(this["GeneralURL"]));
            }
            set {
                this["GeneralURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("settings/gametypes")]
        public string GameTypesURL {
            get {
                return ((string)(this["GameTypesURL"]));
            }
            set {
                this["GameTypesURL"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("90")]
        public int MapChangeTimeoutSeconds {
            get {
                return ((int)(this["MapChangeTimeoutSeconds"]));
            }
            set {
                this["MapChangeTimeoutSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("120")]
        public int DefaultTaskTimeoutSeconds {
            get {
                return ((int)(this["DefaultTaskTimeoutSeconds"]));
            }
            set {
                this["DefaultTaskTimeoutSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Users.dll")]
        public string UsersRelFilePath {
            get {
                return ((string)(this["UsersRelFilePath"]));
            }
            set {
                this["UsersRelFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public int PageNavTimeoutSeconds {
            get {
                return ((int)(this["PageNavTimeoutSeconds"]));
            }
            set {
                this["PageNavTimeoutSeconds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Log.txt")]
        public string Logfile {
            get {
                return ((string)(this["Logfile"]));
            }
            set {
                this["Logfile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("there\'s no password like no password")]
        public string DefaultGamePassword {
            get {
                return ((string)(this["DefaultGamePassword"]));
            }
            set {
                this["DefaultGamePassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ServiceUsername {
            get {
                return ((string)(this["ServiceUsername"]));
            }
            set {
                this["ServiceUsername"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ServicePassword {
            get {
                return ((string)(this["ServicePassword"]));
            }
            set {
                this["ServicePassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ChatId {
            get {
                return ((string)(this["ChatId"]));
            }
            set {
                this["ChatId"] = value;
            }
        }
    }
}
