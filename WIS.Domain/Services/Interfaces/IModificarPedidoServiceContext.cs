using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface IModificarPedidoServiceContext : IServiceContext
    {
        HashSet<short> Rutas { get; set; }
        HashSet<string> Predios { get; set; }
        HashSet<int> Transportadoras { get; set; }
        HashSet<string> TiposAgente { get; set; }
        HashSet<string> TiposPedido { get; set; }
        HashSet<string> TiposExpedicion { get; set; }
        HashSet<string> TiposExpedicionPedido { get; set; }
        HashSet<string> CondicionesLiberacion { get; set; }
        HashSet<string> DetallePedidoLpnAtributos { get; set; }
        HashSet<string> Zonas { get; set; }
        Dictionary<string, string> Agentes { get; set; }
        Dictionary<string, Pedido> Pedidos { get; set; }
        Dictionary<string, Producto> Productos { get; set; }
        Dictionary<string, decimal?> SaldosTotales { get; set; }
        Dictionary<string, bool> EditarTipoHabilitado { get; set; }
        Dictionary<string, DetallePedido> Detalles { get; set; }
        Dictionary<string, DetallePedidoDuplicado> Duplicados { get; set; }
        Dictionary<string, Atributo> Atributos { get; set; }
        Dictionary<string, List<DetallePedidoAtributos>> DetallePedidoAtributos { get; set; }
        Dictionary<string, DetallePedidoLpn> DetallesPedidoLpn { get; set; }
        Dictionary<string, List<DetallePedidoAtributosLpn>> DetallePedidoAtributosLpn { get; set; }
        Dictionary<string, List<string>> DetallesConLoteAsociados { get; set; }
        Dictionary<string, List<string>> DuplicadosAsociados { get; set; }
        Dictionary<string, List<string>> DuplicadosConLoteAsociados { get; set; }

        Task Load();

        bool ExisteAtributo(string nombre);
        bool ExisteAtributoDetalleLpn(string idLpnExterno, string tipoLpn, int empresa, string producto, decimal faixa, string lote, string nombreAtributo);
        bool ExisteCondicionLiberacion(string condicion);
        bool ExisteDetalleLpn(long lpn, string producto, string lote);
        bool ExisteLpn(string idLpnExterno, string tipo);
        bool ExistePedido(string pedido, int empresa, string cliente);
        bool ExistePredio(string predio);
        bool ExisteRelTpExpdicionPedido(string tipoExpedicion, string tipoPedido);
        bool ExisteRuta(short ruta);
        bool ExisteTpExpedicion(string tipo);
        bool ExisteTpPedido(string tipo);
        bool ExisteTransportadora(int transportadora);
        bool ExisteZona(string zona);
        Agente GetAgente(string codigo, int empresa, string tipo);
        Atributo GetAtributo(string nombre);
        DetallePedidoAtributos GetAtributos(DetallePedidoAtributos configuracion);
        DetallePedidoAtributosLpn GetAtributos(DetallePedidoAtributosLpn configuracion);
        List<DetallePedidoAtributosLpn> GetAtributosLpnRegistrados(DetallePedidoLpn detalle);
        List<DetallePedidoAtributos> GetAtributosRegistrados(DetallePedido detalle);
        int GetCantidadDetallesLpn(long numeroLPN);
        decimal GetCantidadDetallesLpn(long numeroLPN, string producto, string lote);
        DetallePedidoLpn GetDetalleLpn(DetallePedidoLpn detalleLpn);
        DetallePedido GetDetallePedido(DetallePedido det);
        DetallePedido GetDetallePedidoNoEspecifico(DetallePedido det);
        List<DetallePedido> GetDetallesConLoteAsociados(DetallePedido det);
        List<LpnDetalle> GetDetallesLpn(long lpn);
        List<DetallePedidoLpn> GetDetallesLpn(Pedido pedido);
        DetallePedidoDuplicado GetDuplicado(DetallePedidoDuplicado dup);
        DetallePedidoDuplicado GetDuplicado(string key);
        List<DetallePedidoDuplicado> GetDuplicadosConLoteAsociados(DetallePedidoDuplicado det);
        string GetIdentificador(string identificador, string producto);
        string GetJson(DetallePedidoAtributos configuracion);
        string GetJson(DetallePedidoAtributosLpn configuracion);
        List<string> GetKeysDuplicadosAsociados(DetallePedido det);
        Lpn GetLpn(string idLpnExterno, string tipo);
        Pedido GetPedido(string pedido, int empresa, string cliente);
        Producto GetProducto(int empresa, string codigo);
        decimal GetSaldoTotal(string pedido, int empresa, string cliente);
        LpnTipo GetTipo(string tipo);
        bool IsPedidoProduccion(string pedido, int empresa, string cliente);
        bool PuedoEditarTipo(string pedido, int empresa, string cliente);
    }
}