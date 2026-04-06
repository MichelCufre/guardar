using System.Collections.Generic;
using WIS.Domain.Documento;
using WIS.Domain.StockEntities;

namespace WIS.Domain.General.API.Bulks
{
    public class PickingBulkOperationContext
    {
        public Dictionary<string, object> NewContenedores = new Dictionary<string, object>();
        public Dictionary<string, object> UpdateContenedores = new Dictionary<string, object>();

        public Dictionary<string, Stock> NewStock = new Dictionary<string, Stock>();
        public Dictionary<string, Stock> UpdateStockBaja = new Dictionary<string, Stock>();
        public Dictionary<string, Stock> UpdateStockAlta = new Dictionary<string, Stock>();

        public List<object> NewDetallesPicking = new List<object>();
        public List<object> UpdateDetallesPicking = new List<object>();
        public List<object> RemoveDetallesPicking = new List<object>();

        public Dictionary<string, DocumentoPreparacionReserva> NewReservasDocumentales = new Dictionary<string, DocumentoPreparacionReserva>();
        public Dictionary<string, DocumentoPreparacionReserva> UpdateReservasDocumentales = new Dictionary<string, DocumentoPreparacionReserva>();
        public Dictionary<string, DocumentoPreparacionReserva> RemoveReservasDocumentales = new Dictionary<string, DocumentoPreparacionReserva>();

        public Dictionary<int, object> UpdateEquipos = new Dictionary<int, object>();
    }
}
