using System.ServiceModel.Channels;

namespace WIS.XmlData.WInterface.Models
{
    public class CMNException : Exception
    {
        public string[] StrArguments { get; set; }
        public int Nivel { get; set; }
        public string Property { get; set; }
        public CMNExceptionList ErrorList;

        public string GetMessage()
        {
            return Message;
        }

        public CMNException(string texto, string[] strArguments = null) : base(texto)
        {
            this.StrArguments = strArguments;
            this.Property = string.Empty;

            ErrorList = new CMNExceptionList();

        }

        public static string GetExceptionMessage(Exception ex)
        {
            string msg = string.Empty;

            if (ex.InnerException != null && ex.InnerException.InnerException != null)
                msg = ex.InnerException.InnerException.Message;

            if (string.IsNullOrEmpty(msg) && ex.InnerException != null)
                msg = ex.InnerException.Message;

            if (string.IsNullOrEmpty(msg))
                msg = ex.Message;

            return msg;
        }
    }
}
