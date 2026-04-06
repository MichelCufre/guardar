using System.Collections.Generic;

namespace WIS.Domain.Documento
{
    public interface IDocumentoActa : IDocumento
    {
        IDocumento InReference { get; set; }
        List<DocumentoLineaEgreso> OutDetail { get; set; }
        List<DocumentoActaDetalle> ActaDetail { get; set; }
        void CalcularValoresCifFob();
        void ValidarActa();
    }
}
