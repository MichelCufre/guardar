using System.Collections.Generic;
using WIS.Documento.Execution;
using WIS.Domain.DataModel;
using WIS.Domain.Documento;
using WIS.Domain.Produccion;
using WIS.Domain.Recepcion;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface IDocumentoService
    {
        Agenda CrearAgenda(IUnitOfWork uow, IDocumentoIngreso documento, string descDocumentoAduanero, string nroDocumentoAduanero);
        List<Agenda> CrearAgenda(IUnitOfWork uow, IDocumentoAgrupador documento);
        void ValidarCancelacionIngreso(IUnitOfWork uow, IDocumentoAgrupador documento);
        void HabilitarCargaYCierreCamion(IUnitOfWork uow, IDocumentoEgreso documento);
        ReservaDocumentalResponse DesreservarStock(IUnitOfWork uow, List<Stock> stockReservado);
        ProduccionDocumentalResponse CrearDocumentos(IUnitOfWork uow, IngresoWhiteBox produccion);
        CambioLoteResponse CambiarLote(CambioLoteRequest request);
        ProduccionDocumentalResponse DocumentarProduccion(ProduccionDocumentalRequest request);
        TransferenciaDocumentalResponse DocumentarTransferencia(TransferenciaDocumentalRequest request);
        TransferenciaDocumentalResponse DocumentarTransferenciaSinPreparacion(TransferenciaDocumentalRequest request);
        ReservaDocumentalResponse AfectarReservaDocumental(ReservaDocumentalRequest request);
        ModificarReservaDocumentalResponse ModificarReservaDocumental(ModificarReservaDocumentalRequest request);
    }
}
