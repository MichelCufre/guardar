using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models
{
    public class ServerResponse
    {
        public string Data { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string[] MessageArguments { get; set; }
        public string PageToken { get; set; }
        public bool TooManySessions { get; set; }

        public ServerResponse()
        {

        }

        public ServerResponse(string data)
        {
            this.Data = data;
            this.Status = "OK";
        }

        public ServerResponse(string data, string status, string message, string[] arguments = default)
        {
            this.Data = data;
            this.Status = status;
            this.Message = message;
            this.MessageArguments = default;
        }

        public void SetError(string message, string[] arguments = default)
        {
            this.Status = "ERROR";
            this.Message = message;
            this.MessageArguments = arguments;
        }

        public void SetOk()
        {
            this.Status = "OK";
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
