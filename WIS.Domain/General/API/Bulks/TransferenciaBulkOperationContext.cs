using System.Collections.Generic;
using WIS.Domain.StockEntities;

namespace WIS.Domain.General.API.Bulks
{
    public class TransferenciaBulkOperationContext
    {
        public object NewPalletTransferencia = new object();
        public object UpdatePalletTransferencia = new object();

        public List<object> NewDetallesPallet = new List<object>();
        public List<object> UpdateDetallesPallet = new List<object>();

        public Dictionary<string, Stock> UpdateStockOrigenBaja = new Dictionary<string, Stock>();
        public Dictionary<string, Stock> NewStockEquipo = new Dictionary<string, Stock>();
        public Dictionary<string, Stock> UpdateStockEquipoAlta = new Dictionary<string, Stock>();
        public Dictionary<string, Stock> UpdateStockEquipoBaja = new Dictionary<string, Stock>();
        public Dictionary<string, Stock> NewStockDestino = new Dictionary<string, Stock>();
        public Dictionary<string, Stock> UpdateStockDestinoAlta = new Dictionary<string, Stock>();
    }
}
