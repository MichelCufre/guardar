using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Documento.Integracion.Recepcion;

namespace WIS.Domain.Documento
{
    public interface IDocumentoIngreso : IDocumento
    {
        bool Balanceado { get; set; }

        bool IsHabilitadoParaBalanceo(IUnitOfWork uow);
        DocumentoLineaBalanceo BalancearLote(List<InformacionBalanceo> InfoBalanceo);
        void ConfirmarBalanceo();
        void CalcularValoresCifFob();
    }
}
