using System.Collections.Generic;

namespace WIS.Domain.Documento.Serializables
{
    public class DocumentoTipoResponse
    {
        public List<DocumentoTipoFilaResponse> TiposDocumento { get; set; }
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }

        public virtual void AddFila(string TipoDocumento, string TipoOperacion, string DescripcionTipoDocumento, bool NumeroAutogenerado, bool IngresoManual)
        {
            TiposDocumento.Add(new DocumentoTipoFilaResponse()
            {
                DescripcionTipoDocumento = DescripcionTipoDocumento,
                TipoDocumento = TipoDocumento,
                TipoOperacion = TipoOperacion,
                IngresoManual = IngresoManual,
                NumeroAutogenerado = NumeroAutogenerado
            });
        }
    }

    public class DocumentoTipoFilaResponse
    {
        public string TipoDocumento { get; set; }
        public string TipoOperacion { get; set; }
        public string DescripcionTipoDocumento { get; set; }
        public bool NumeroAutogenerado { get; set; }
        public bool IngresoManual { get; set; }
    }
}