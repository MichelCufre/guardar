
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Exceptions;

namespace WIS.WebApplication.Models
{
    public class ServerRequest
    {
        public string Application { get; set; }
        public string Data { get; set; }
        public string ComponentId { get; set; }
        public string PageToken { get; set; }

        public ServerRequest()
        {

        }

        public ServerRequest(string application, string data)
        {
            this.Application = application;
            this.Data = data;
        }

        public string GetBaseApplication()
        {
            //TODO: Ver si pasar a dependency injection o strategy pattern
            if (string.IsNullOrEmpty(this.Application) || this.Application.Length < 3)
                throw new MissingParameterException("General_Sec0_Error_Error09");

            return this.Application.Substring(0, 3);
        }
    }
}
