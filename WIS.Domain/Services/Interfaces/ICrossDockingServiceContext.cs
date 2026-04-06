using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Dtos;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface ICrossDockingServiceContext : IServiceContext
    {
        HashSet<string> Predios { get; set; }
        HashSet<string> TiposLpn { get; set; }
        List<Stock> Stocks { get; set; }
        List<Contenedor> ContenedoresActivos { get; set; }
        List<AgendaDetalle> DetallesAgenda { get; set; }
        List<EtiquetaEnUso> EtiquetasEnUso { get; set; }
        List<PuertaEmbarque> PuertasEmbarque { get; set; }
        List<TipoContenedor> TiposContenedor { get; set; }
        List<EtiquetaLoteDetalle> EtiquetaLoteDetalle { get; set; }
        Dictionary<string, Agenda> Agendas { get; set; }
        Dictionary<string, Agente> Clientes { get; set; }
        Dictionary<string, Producto> Productos { get; set; }
        Dictionary<string, CrossDockingEnUnaFase> CrossDockingActivos { get; set; }
        Dictionary<string, DetallePendienteCrossDocking> DetallesPendienteCrossDocking { get; set; }

        Task Load();

        bool AnyCrossDocking(int agenda, int preparacion);
        bool EsTipoLpn(string tipoContenedor);
        bool ExistePredio(string predio);
        Agenda GetAgenda(int nuAgenda, int empresa);
        Agente GetCliente(string tipoAgente, string codigoAgente, int empresa);
        Contenedor GetContenedor(string idExternoContenedor, string tipoContenedor);
        AgendaDetalle GetDetalle(int numeroAgenda, string producto, int empresa, decimal faixa, string identificador);
        EtiquetaLoteDetalle GetDetallesEtiquetaLote(int etiquetaLote, string producto, int empresa, decimal faxia, string identificador);
        EtiquetaEnUso GetEtiquetaEnUso(string numeroExterno, string tipoEtiqueta);
        Producto GetProducto(int empresa, string codigo);
        PuertaEmbarque GetPuerta(short codigoPuerta);
        PuertaEmbarque GetPuerta(string Ubicacion);
        Stock GetStock(string cdEndereco, string cdProducto, int cdEmpresa, string nuIdentificador, decimal cdFaixa);
        TipoContenedor GetTipoContenedor(string tpContenedor);
        DetallePendienteCrossDocking SaldoPendienteXd(int agenda, int preparacion, string cliente, int empresa, string producto, string identificador);
    }
}