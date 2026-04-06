using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface ITransferenciaStockServiceContext : IServiceContext
    {
        public string UbicacionEquipo { get; set; }
        public HashSet<string> Predios { get; set; }
        public HashSet<Stock> Stocks { get; set; }
        public List<Stock> CantidadesLpns { get; set; }
        public Dictionary<string, Producto> Productos { get; set; }
        public Dictionary<string, Ubicacion> Ubicaciones { get; set; }
        public Dictionary<short, UbicacionArea> Areas { get; set; }
        public Dictionary<short, UbicacionTipo> TiposUbicacion { get; set; }
        public Dictionary<string, UbicacionPickingProducto> PickingsProducto { get; set; }

        Task Load();

        bool AnyProductoLoteUbicacion(string ubicacion, string producto, int empresa, string lote);
        bool AnyProductoUbicacion(string ubicacion, string producto, int empresa);
        bool ExisteAsignacionPicking(string ubicacion, string producto, int empresa);
        bool ExisteUbicacion(string key);
        UbicacionArea GetArea(short area);
        decimal GetCantidadDisponible(Stock sto);
        decimal GetCantidadDisponibleLpn(string ubicacion, string producto, int empresa, string identificador, decimal faixa);
        string GetPredioOperacion();
        Producto GetProducto(string codigo, int empresa);
        Stock GetStock(string ubicacion, string producto, int empresa, string identificador, decimal faixa);
        UbicacionTipo GetTipoUbicacion(short tipo);
        Ubicacion GetUbicacion(string ubicacion);
        DateTime? GetVencimientoStock(TransferenciaStock data, bool destino = false);
        bool PredioUnico();
        void SetUbicacionEquipo();
        bool UbicacionEquipoValida();
    }
}