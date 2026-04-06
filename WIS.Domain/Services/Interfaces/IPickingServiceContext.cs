using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.Documento;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface IPickingServiceContext : IServiceContext
    {
        HashSet<Stock> Stocks { get; set; } 
        HashSet<string> Predios { get; set; } 
        HashSet<string> TiposAgente { get; set; } 
        HashSet<string> TiposContenedor { get; set; } 
        HashSet<string> TiposLpn { get; set; } 
        HashSet<SuperClase> SubClases { get; set; } 
        HashSet<CargaCamion> CargasCamion { get; set; } 
        List<DetallePreparacion> DetallesPendientes { get; set; } 
        HashSet<PedidoContenedor> PedidosContenedor { get; set; } 
        HashSet<DetallePreparacion> DetallesPreparacionDestino { get; set; } 

        Dictionary<string, Pedido> Pedidos { get; set; } 
        Dictionary<string, Agente> Clientes { get; set; } 
        Dictionary<string, Producto> Productos { get; set; } 
        Dictionary<string, Contenedor> Contenedores { get; set; } 
        Dictionary<long, short> CargasRutas { get; set; } 
        Dictionary<int, Carga> PreparacionOrigenCarga { get; set; } 
        Dictionary<int, Carga> PreparacionDescargaCarga { get; set; } 
        Dictionary<short, UbicacionArea> Areas { get; set; } 
        Dictionary<int, Preparacion> Preparaciones { get; set; } 
        Dictionary<string, Ubicacion> Ubicaciones { get; set; } 
        Dictionary<string, UbicacionEquipo> UbicacionesEquipoPredio { get; set; } 
        Dictionary<int, List<DocumentoPreparacionReserva>> ReservasDocumentales { get; set; }

        Task Load();

        bool EsTipoLpn(string tipoContenedor);
        bool ExisteCarga(long nroCarga);
        bool ExisteClienteDistintoContenedor(int preparacion, int contenedor, string cliente);
        bool ExistePedido(string pedido, int empresa, string cliente);
        bool ExisteRutaDistintaContenedor(int preparacion, int contenedor, int ruta);
        bool ExisteTipoAgente(string tipoAgente);
        bool ExisteTipoContenedor(string tipoContenedor);
        bool ExisteUbicacion(string key);
        Agente GetAgente(string codigo, int empresa, string tipo);
        string GetAgrupacion(int preparacion);
        decimal GetCantidadPendiente(DetallePreparacion pick, string agrupacion);
        CargaCamion GetCargaCamion(long? carga);
        Contenedor GetContenedor(string idExterno, string tipoContenedor);
        IEnumerable<DetallePreparacion> GetDetallesPendiente(DetallePreparacion pick, string agrupacion);
        long? GetNroCarga(int preparacion, bool origen);
        Pedido GetPedido(string pedido, int empresa, string cliente);
        Preparacion GetPreparacion(int preparacion);
        Producto GetProducto(string codigo, int empresa);
        IEnumerable<DocumentoPreparacionReserva> GetReservasDetalles(int preparacion, int empresa, string producto, string identificador, decimal faixa);
        DocumentoPreparacionReserva GetReservasDocumento(string nroDocumento, string tpDocumento, int preparacion, int empresa, string producto, string identificador, decimal faixa);
        short GetRuta(long nroCarga);
        Stock GetStock(string ubicacion, int empresa, string producto, string identificador, decimal faixa);
        SuperClase GetSubClaseProducto(string clase);
        Ubicacion GetUbicacion(string ubicacion);
        UbicacionEquipo GetUbicacionEquipo(string predio);
        bool ManejaDocumental();
        bool PuedeCompartirContenedorPicking(string cliente, int contenedor, string valorCompatibilidad);
    }
}