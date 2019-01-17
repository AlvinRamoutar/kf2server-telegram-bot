using System.Collections.Generic;

namespace kf2server_tbot.Utils {

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
