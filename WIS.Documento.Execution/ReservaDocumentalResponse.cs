using System.Collections.Generic;

namespace WIS.Documento.Execution
{
    public class ReservaDocumentalResponse
    {
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }

        public ReservaDocumentalResponse()
        {
            this.Success = true;
        }
    }
}
