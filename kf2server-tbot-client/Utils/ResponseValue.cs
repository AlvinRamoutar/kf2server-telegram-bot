using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace kf2server_tbot_client.Utils {

    [DataContract]
    public class ResponseValue {

        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public Dictionary<string, string> Data { get; set; }
        
        public ResponseValue(bool isSuccess, string message, Dictionary<string, string> data) {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        [OperationContract]
        public override string ToString() {
            return string.Format("{{ IsSuccess:{0}, Message=\"{1}\", Data:{2} }}", IsSuccess, Message, Data);
        }

    }
}
