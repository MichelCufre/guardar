using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface IPedidoServiceContext : IServiceContext
    {
        HashSet<int> Transportadoras { get; set; }
        HashSet<short> Rutas { get; set; }
        HashSet<string> Predios { get; set; }
        HashSet<string> TiposAgente { get; set; }
        HashSet<string> TiposPedido { get; set; }
        HashSet<string> TiposExpedicion { get; set; }
        HashSet<string> TiposExpedicionPedido { get; set; }
        HashSet<string> CondicionesLiberacion { get; set; }
        HashSet<string> DetallePedidoLpnAtributos { get; set; }
        HashSet<string> Zonas { get; set; }
        Dictionary<string, Lpn> Lpns { get; set; }
        Dictionary<string, string> Agentes { get; set; }
        Dictionary<string, Pedido> Pedidos { get; set; }
        Dictionary<string, LpnTipo> TiposLpn { get; set; }
        Dictionary<string, Producto> Productos { get; set; }
        Dictionary<string, Atributo> Atributos { get; set; }
        Dictionary<long, List<LpnDetalle>> DetallesLpn { get; set; }

        Task Load();

        bool ExisteAtributo(string nombre);
        bool ExisteAtributoDetalleLpn(string idLpnExterno, string tipoLpn, int empresa, string producto, decimal faixa, string lote, string nombreAtributo);
        bool ExisteCondicionLiberacion(string condicion);
        bool ExisteDetalleLpn(long nroLpn, string producto, string lote);
        bool ExisteLpn(string idLpnExterno, string tipo);
        bool ExistePedido(string pedido, int empresa, string cliente);
        bool ExistePredio(string predio);
        bool ExisteRelTpExpdicionPedido(string tipoExpedicion, string tipoPedido);
        bool ExisteRuta(short ruta);
        bool ExisteTpExpedicion(string tipo);
        bool ExisteTpPedido(string tipo);
        bool ExisteTransportista(int transportadora);
        bool ExisteZona(string zona);
        Agente GetAgente(string codigo, int empresa, string tipo);
        Atributo GetAtributo(string nombre);
        int GetCantidadDetallesLpn(long nroLpn);
        decimal GetCantidadDetallesLpn(long nroLpn, string producto, string lote);
        List<LpnDetalle> GetDetallesLpn(long nroLpn);
        string GetIdentificador(string identificador, string producto);
        Lpn GetLpn(string idLpnExterno, string tipo);
        Producto GetProducto(int empresa, string codigo);
        LpnTipo GetTipoLpn(string tipo);
    }
}