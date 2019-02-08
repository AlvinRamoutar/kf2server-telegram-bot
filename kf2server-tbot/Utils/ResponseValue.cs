using System.Collections.Generic;

/// <summary>
/// KF2 Telegram Bot
/// An experiment in automating KF2 server webmin actions with Selenium, triggered via Telegram's Bot API
/// Copyright (c) 2018-2019 Alvin Ramoutar https://alvinr.ca/ 
/// </summary>
namespace kf2server_tbot.Utils {

    /// <summary>
    /// Container for Commander method data.
    /// <para>Not a struct, since it should be nullable for long performing commands.</para>
    /// </summary>
    public class ResponseValue {

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public Dictionary<string, string> Data { get; set; }
        
        public ResponseValue(bool isSuccess, string message, Dictionary<string, string> data) {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public override string ToString() {
            return string.Format("{{ IsSuccess:{0}, Message=\"{1}\", Data:{2} }}", IsSuccess, Message, Data);
        }

    }
}
