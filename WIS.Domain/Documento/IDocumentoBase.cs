using System.Collections.Generic;

namespace WIS.Domain.Documento
{
    public interface IDocumentoBase
    {
        string Numero { get; set; }
        string Tipo { get; set; }
        List<DocumentoLinea> Lineas { get; set; }
    }
}
