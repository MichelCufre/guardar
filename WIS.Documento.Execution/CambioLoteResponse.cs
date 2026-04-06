using System.Collections.Generic;

namespace WIS.Documento.Execution
{
    public class CambioLoteResponse
    {
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }
        public List<DocumentoCambioLoteResponse> Documentos { get; set; }

        public CambioLoteResponse()
        {
            Success = true;
            Documentos = new List<DocumentoCambioLoteResponse>();
        }
    }
}
