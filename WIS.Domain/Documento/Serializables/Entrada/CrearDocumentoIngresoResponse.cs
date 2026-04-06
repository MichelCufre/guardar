using System.Collections.Generic;

namespace WIS.Domain.Documento.Serializables.Entrada
{
    public class CrearDocumentoIngresoResponse
    {
        public CrearDocumentoIngresoResponse()
        {
            this.Success = false;
            this.Errors = new List<Error>();
        }

        public bool Success { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public Error(string campo, string valor, string descError, string registro = "1")
        {
            this.CAMPO = campo;
            this.VL_CAMPO = valor;
            this.DS_ERROR = descError;
            this.NU_REGISTRO = registro;
        }

        public string CAMPO { get; set; }
        public string VL_CAMPO { get; set; }
        public string DS_ERROR { get; set; }
        public string NU_REGISTRO { get; set; }
    }
}
