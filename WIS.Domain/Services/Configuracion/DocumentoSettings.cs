using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Services.Configuracion
{
    public class DocumentoSettings
    {
        public const string Position = "DocumentoSettings";

        public string ReservaDocumentalEndpoint { get; set; }
        public string CrearDocumentosEndpoint { get; set; }

    }
}
