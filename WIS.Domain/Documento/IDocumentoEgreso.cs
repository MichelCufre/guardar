using System.Collections.Generic;

namespace WIS.Domain.Documento
{
    public interface IDocumentoEgreso : IDocumento
    {
        List<DocumentoLineaEgreso> OutDetail { get; set; }
    }
}
