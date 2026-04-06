using System.Runtime.Serialization;

namespace WIS.XmlData.WInterface.Models
{
    [DataContract]
    public class CSTransfer
    {
        [DataMember]
        public CSResult status { get; set; }
        [DataMember]
        public string jsonData { get; set; }
        [DataMember]
        public string errorMessage { get; set; }
        [DataMember]
        public ECSBackEndMethods? invokeBackEndMethod { get; set; }

        public CSTransfer()
        {
            status = CSResult.OK;
            errorMessage = string.Empty;
        }

        public string toLog()
        {
            return "status: " + status.ToString() + " - " + "jsonData: " + jsonData + " - " + "errorMessage: " + errorMessage;
        }
    }
}
