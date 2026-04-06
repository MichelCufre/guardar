using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAjustesDeStockServiceContext : IServiceContext
    {
        public HashSet<string> Predios { get; set; }
        public List<Stock> Stocks { get; set; }
        public List<Stock> CantidadesLpns { get; set; }
        public HashSet<Stock> SeriesExistentes { get; set; }
        public Dictionary<string, Producto> Productos { get; set; } 
        public Dictionary<string, Ubicacion> Ubicaciones { get; set; } 
        public Dictionary<short, UbicacionArea> Areas { get; set; } 
        public Dictionary<string, MotivoAjuste> MotivosAjuste { get; set; } 
        public Dictionary<short, UbicacionTipo> TiposUbicacion { get; set; } 
        public Dictionary<string, UbicacionPickingProducto> UbicacionPickingProducto { get; set; } 

        Task Load();
        
        bool AnyMotivoAjuste(string cdMotivoAjuste);
        bool AnyProductoEnUbicacion(string cdEndereco, string cdProducto, int cdEmpresa);
        bool AnyProductoLoteEnUbicacion(string cdEndereco, string cdProducto, int cdEmpresa, string lote);
        bool ExistePredio(string predio);
        bool ExisteSerie(string codigoProducto, string identificador);
        bool ExisteStock(AjusteStock data);
        UbicacionArea GetAreaUbic(short idUbicacionArea);
        decimal GetCantidadSuelta(AjusteStock data);
        decimal GetCantidadDisponibleLpn(AjusteStock data);
        Producto GetProducto(int empresa, string codigo);
        Stock GetStock(AjusteStock data);
        UbicacionTipo GetTipoUbicacion(short idUbicacionTipo);
        Ubicacion GetUbicacion(string ubicacion);
        UbicacionPickingProducto GetUbicacionPickingProducto(string cdEndereco, string cdProducto, int cdEmpresa);
    }
}