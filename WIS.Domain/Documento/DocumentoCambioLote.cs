using System.Collections.Generic;

namespace WIS.Domain.Documento
{
    public class DocumentoCambioLote : IDocumentoBase
    {
        public string Tipo { get; set; }
        public string Numero { get; set; }
        public List<DocumentoLinea> Lineas { get; set; }

        public DocumentoCambioLote(string tipo, string numero)
        {
            Tipo = tipo;
            Numero = numero;
            Lineas = new List<DocumentoLinea>();
        }
    }
}
