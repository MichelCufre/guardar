using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using static WIS.Domain.Services.EgresoServiceContext;

namespace WIS.Domain.Services.Interfaces
{
    public interface IEgresoServiceContext : IServiceContext
    {
        KeyDetalles KeysDetallesEgreso { get; set; }
        List<Predio> Predios { get; set; }
        HashSet<int> Empresas { get; set; }
        List<PuertaEmbarque> Puertas { get; set; }
        HashSet<int> Transportistas { get; set; }
        HashSet<string> NuPredios { get; set; }
        HashSet<string> IdsExternos { get; set; }
        HashSet<string> NuPrediosExternos { get; set; }
        HashSet<string> CargasEnOtroCamion { get; set; }
        HashSet<Carga> CargasCompartidasPedidos { get; set; }
        HashSet<Carga> CargasCompartidasContenedores { get; set; }
        Dictionary<short, Ruta> Rutas { get; set; }
        Dictionary<string, Agente> Agentes { get; set; }
        Dictionary<int, Vehiculo> Vehiculos { get; set; }
        Dictionary<long, Carga> CargasHabilitadas { get; set; }
        Dictionary<string, Pedido> PedidosHabilitados { get; set; }
        Dictionary<string, Pedido> PedidosConPendientes { get; set; }
        Dictionary<string, Contenedor> ContenedoresHabilitados { get; set; }
        Dictionary<string, List<DetallePreparacion>> PedidosCargasLiberadas { get; set; }
        Dictionary<string, List<ContenedorExternoCarga>> ContenedoresCargasLiberadas { get; set; }

        Task Load();

        bool CargaEnOtroCamion(string cargaCliente);
        bool ExisteEmpresa(int cdEmpresa);
        bool ExisteIdExterno(string idExterno);
        bool ExistePredio(string predio);
        bool ExistePredioExterno(string idExterno);
        bool ExistePuerta(short puerta);
        bool ExisteTransportista(int transportadora);
        Agente GetAgente(string codigo, int empresa, string tipo);
        Carga GetCargaHabilitada(long nuCarga);
        Contenedor GetContenedorHabilitado(string idExternoContenedor, string tipoContenedor, int empresa);
        Pedido GetPedidoHabilitado(string pedido, int empresa, string cliente);
        PuertaEmbarque GetPuerta(short puerta);
        Ruta GetRuta(short cdRuta);
        Vehiculo GetVehiculo(int cdVehiculo);
        void ProcesoAgentes();
        void ProcesoCargas(Dictionary<long, Carga> keysCargas, bool validacionPicking);
        void ProcesoCargasCamion(Dictionary<string, CargaCamion> keysParcialCargaCamion);
        void ProcesoClaves(Dictionary<long, Carga> keysCargas, Dictionary<string, Pedido> keysPedidos, Dictionary<string, Contenedor> keysContenedores, Dictionary<string, CargaCamion> keysParcialCargaCamion, out bool validacionCargaPicking);
        void ProcesoContenedores(Dictionary<string, Contenedor> keysContenedores, Dictionary<string, CargaCamion> keysParcialCargaCamion);
        void ProcesoPedidos(Dictionary<string, Pedido> keysPedidos, Dictionary<string, CargaCamion> keysParcialCargaCamion);
    }
}