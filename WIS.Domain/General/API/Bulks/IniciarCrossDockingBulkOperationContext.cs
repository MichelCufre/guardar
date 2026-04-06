using System.Collections.Generic;
using WIS.Domain.Documento;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;

namespace WIS.Domain.General.API.Bulks
{
    public class IniciarCrossDockingBulkOperationContext
    {
        public List<LineaCrossDocking> NewDetalleCrossDocking;

        public List<DetallePedido> UpdateDetallePedido;

        public List<AgendaDetalle> UpdateDetalleAgenda;

        public List<CrossDockingTemp> RemoveDetalleCrossDockingTemporal;

        public List<DocumentoPreparacionReserva> NewDocumentoPreparacionReserva;

        public List<DocumentoLineaDesafectada> UpdateDocumentoLineaDesafectada;

        public (string NuDocumento, string TipoDocumento, List<DocumentoLinea> DetallesDocumento) UpdateDetallesDocumento;


        public IniciarCrossDockingBulkOperationContext()
        {
            UpdateDetallePedido = new List<DetallePedido>();
            UpdateDetalleAgenda = new List<AgendaDetalle>();
            NewDetalleCrossDocking = new List<LineaCrossDocking>();
            RemoveDetalleCrossDockingTemporal = new List<CrossDockingTemp>();
            UpdateDocumentoLineaDesafectada = new List<DocumentoLineaDesafectada>();
            NewDocumentoPreparacionReserva = new List<DocumentoPreparacionReserva>();
            UpdateDetallesDocumento = (string.Empty, string.Empty, new List<DocumentoLinea>());
        }

        public virtual bool IsUpdateDetallesDocumentoValid()
        {
            return UpdateDetallesDocumento.NuDocumento != null &&
                   UpdateDetallesDocumento.TipoDocumento != null &&
                   UpdateDetallesDocumento.DetallesDocumento != null;
        }
    }
}