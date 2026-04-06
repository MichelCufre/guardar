using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Services.Configuracion
{
    public class WmsApiSettings
    {
        public const string Position = "WmsApiSettings";
        public string Endpoint { get; set; }
        public bool IsEnabled { get; set; }
    }
}
