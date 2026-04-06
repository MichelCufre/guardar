using System.Collections.Generic;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Produccion;
using WIS.Domain.StockEntities;

namespace WIS.Domain.General.API.Bulks
{
    public class ConsumirProduccionBulkOperationContext
    {
        public Dictionary<string, Stock> UpdateStocks = new Dictionary<string, Stock>();
        public List<AjusteStock> NewAjustesStocks = new List<AjusteStock>();

        public List<IngresoProduccionDetalle> NewIngresoProduccionDetalleMov = new List<IngresoProduccionDetalle>();
        public Dictionary<string, IngresoProduccionDetalleReal> NewIngresoProduccionDetallesReales = new Dictionary<string, IngresoProduccionDetalleReal>();
        public Dictionary<long, IngresoProduccionDetalleReal> UpdateIngresoProduccionDetallesReales = new Dictionary<long, IngresoProduccionDetalleReal>();

        public IngresoProduccion Ingreso;
        public EspacioProduccion EspacioProduccion;
        public bool NotificarProduccion { get; set; }
    }
}
