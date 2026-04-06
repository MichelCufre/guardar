using System.Collections.Generic;
using WIS.Domain.Documento;
using WIS.Domain.Documento.TiposDocumento;
using WIS.Domain.General;

namespace WIS.Domain.Picking.Logic.Bulks
{
    public class TraspasoBulkOperationContext
    {
        public List<DocumentoPreparacionReserva> DetallePreparacionReservaDocumental = new List<DocumentoPreparacionReserva>();

        public List<DetallePreparacion> DetallePreparacion = new List<DetallePreparacion>();

        public List<DetallePreparacionLpn> DetallePreparacionLpn { get; set; }
        public DocumentoEgreso DocumentoSalida { get; set; }
        public DocumentoIngreso DocumentoEntrada { get; set; }
        public Preparacion PreparacionDestino { get; set; }
        public Carga CargaDestino { get; set; }
        public List<Contenedor> ContenedoresDestino { get; set; }

        public List<AjusteStock> AjustesStock = new List<AjusteStock>();
    }
}
