using System.Collections.Generic;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Models;
using WIS.Domain.StockEntities;

namespace WIS.Domain.General.API.Bulks
{
    public class ProducirProduccionBulkOperationContext
    {
        public Dictionary<string, Stock> NewStocks = new Dictionary<string, Stock>();
        public Dictionary<string, Stock> UpdateStocks = new Dictionary<string, Stock>();
        public List<AjusteStock> NewAjustesStocks = new List<AjusteStock>();

        public List<SalidaProduccionDetalle> NewSalidaProduccionDetalleMov = new List<SalidaProduccionDetalle>();
        public Dictionary<string, IngresoProduccionDetalleSalida> NewSalidaProduccionDetalleReales = new Dictionary<string, IngresoProduccionDetalleSalida>();
        public Dictionary<string, IngresoProduccionDetalleSalida> UpdateSalidaProduccionDetalleReales = new Dictionary<string, IngresoProduccionDetalleSalida>();
        public List<IngresoProduccionDetalleReal> UpdateInsumos = new List<IngresoProduccionDetalleReal>();

        public IngresoProduccion Ingreso;
        public EspacioProduccion EspacioProduccion;
        public bool NotificarProduccion { get; set; }
    }
}
