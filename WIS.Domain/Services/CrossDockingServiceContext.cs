using WIS.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Dtos;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services
{
    public class CrossDockingServiceContext : ServiceContext, ICrossDockingServiceContext
    {
        protected List<CrossDockingUnaFase> _detalles = new List<CrossDockingUnaFase>();

        public HashSet<string> Predios { get; set; } = new HashSet<string>();
        public HashSet<string> TiposLpn { get; set; } = new HashSet<string>();
        public List<Stock> Stocks { get; set; } = new List<Stock>();
        public List<Contenedor> ContenedoresActivos { get; set; } = new List<Contenedor>();
        public List<AgendaDetalle> DetallesAgenda { get; set; } = new List<AgendaDetalle>();
        public List<EtiquetaEnUso> EtiquetasEnUso { get; set; } = new List<EtiquetaEnUso>();
        public List<PuertaEmbarque> PuertasEmbarque { get; set; } = new List<PuertaEmbarque>();
        public List<TipoContenedor> TiposContenedor { get; set; } = new List<TipoContenedor>();
        public List<EtiquetaLoteDetalle> EtiquetaLoteDetalle { get; set; } = new List<EtiquetaLoteDetalle>();
        public Dictionary<string, Agenda> Agendas { get; set; } = new Dictionary<string, Agenda>();
        public Dictionary<string, Agente> Clientes { get; set; } = new Dictionary<string, Agente>();
        public Dictionary<string, Producto> Productos { get; set; } = new Dictionary<string, Producto>();
        public Dictionary<string, CrossDockingEnUnaFase> CrossDockingActivos { get; set; } = new Dictionary<string, CrossDockingEnUnaFase>();
        public Dictionary<string, DetallePendienteCrossDocking> DetallesPendienteCrossDocking { get; set; } = new Dictionary<string, DetallePendienteCrossDocking>();

        public CrossDockingServiceContext(IUnitOfWork uow, List<CrossDockingUnaFase> detalles, int userId, int empresa) : base(uow, userId, empresa)
        {
            _detalles = detalles;
        }

        public async override Task Load()
        {
            var keysProducto = new Dictionary<string, Producto>();
            var keysAgenda = new Dictionary<int, Agenda>();
            var keysCliente = new Dictionary<string, Agente>();
            var keysStock = new Dictionary<string, Stock>();

            var keysContenedores = new Dictionary<string, Contenedor>();
            await base.Load();

            foreach (var p in _uow.PredioRepository.GetPrediosUsuario(UserId))
            {
                Predios.Add(p.Numero);
            }

            foreach (var p in _detalles)
            {
                var keyProducto = $"{p.Producto}.{p.Empresa}";
                if (!keysProducto.ContainsKey(keyProducto))
                    keysProducto[keyProducto] = new Producto() { Codigo = p.Producto.Truncate(40), CodigoEmpresa = p.Empresa };

                if (!keysAgenda.ContainsKey(p.Agenda))
                    keysAgenda[p.Agenda] = new Agenda() { Id = p.Agenda };

                var keyCont = $"{p.IdExternoContenedor}.{p.TipoContenedor}";
                if (!keysContenedores.ContainsKey(keyCont))
                    keysContenedores[keyCont] = new Contenedor() { IdExterno = p.IdExternoContenedor.Truncate(50), TipoContenedor = p.TipoContenedor.Truncate(10) };

                var keyCliente = $"{p.CodigoAgente}.{p.TipoAgente}.{p.Empresa}";
                if (!keysCliente.ContainsKey(keyCliente))
                    keysCliente[keyCliente] = new Agente() { Codigo = p.CodigoAgente.Truncate(40), Tipo = p.TipoAgente.Truncate(3), Empresa = p.Empresa };

                var keyStock = $"{p.Ubicacion}.{p.Producto}.{p.Empresa}.{p.Identificador}.{p.Faixa?.ToString("#.###")}";
                if (!keysStock.ContainsKey(keyStock))
                    keysStock[keyStock] = new Stock() { Ubicacion = p.Ubicacion.Truncate(40), Producto = p.Producto.Truncate(40), Empresa = p.Empresa, Identificador = p.Identificador.Truncate(40), Faixa = p.Faixa ?? 1 };
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProducto.Values))
            {
                var keyProducto = $"{p.Codigo}.{p.CodigoEmpresa}";
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);
                Productos[keyProducto] = p;
            }

            foreach (var p in _uow.AgendaRepository.GetAgenda(keysAgenda.Values))
            {
                var key = $"{p.Id}.{p.IdEmpresa}";
                Agendas[key] = p;
            }

            foreach (var p in _uow.CrossDockingRepository.GetCrossDockingAgendaActivos(keysAgenda.Values))
            {
                var key = $"{p.Agenda}.{p.Preparacion}";
                CrossDockingActivos[key] = p;
            }

            foreach (var p in _uow.AgenteRepository.GetAgentes(keysCliente.Values))
            {
                var keyCliente = $"{p.Codigo}.{p.Tipo}.{p.Empresa}";
                Clientes[keyCliente] = p;
            }

            foreach (var p in _uow.AgendaRepository.SaldoPendienteXd(keysAgenda.Values))
            {
                string key = $"{p.Id}.{p.Empresa}.{p.Cliente}.{p.Producto}.{p.Identificador}";
                DetallesPendienteCrossDocking[key] = p;
            }

            DetallesAgenda = _uow.AgendaRepository.GetDetalleAgenda(keysAgenda.Values).ToList();
            TiposContenedor = _uow.ContenedorRepository.GetTiposContenedores();
            PuertasEmbarque = _uow.PuertaEmbarqueRepository.GetPuertasActivas();
            ContenedoresActivos = _uow.ContenedorRepository.GetContenedoresActivos(keysContenedores.Values);
            EtiquetasEnUso = _uow.EtiquetaLoteRepository.GetEtiquetaLoteEnUso(keysContenedores.Values);
            EtiquetaLoteDetalle = _uow.EtiquetaLoteRepository.GetEtiquetaLoteDetalle(EtiquetasEnUso);
            Stocks = _uow.StockRepository.GetStocks(keysStock.Values).ToList();
            TiposLpn = _uow.ManejoLpnRepository.GetTiposLPN().Select(p => p.Tipo).ToHashSet();

        }

        public virtual bool ExistePredio(string predio)
        {
            return Predios.Contains(predio);
        }

        public virtual Agenda GetAgenda(int nuAgenda, int empresa)
        {
            var key = $"{nuAgenda}.{empresa}";
            return Agendas.GetValueOrDefault(key, null);
        }

        public virtual PuertaEmbarque GetPuerta(short codigoPuerta)
        {
            return PuertasEmbarque.FirstOrDefault(x => x.Id == codigoPuerta);
        }

        public virtual PuertaEmbarque GetPuerta(string Ubicacion)
        {
            return PuertasEmbarque.FirstOrDefault(x => x.CodigoUbicacion == Ubicacion);
        }

        public virtual Producto GetProducto(int empresa, string codigo)
        {
            var keyProducto = $"{codigo}.{empresa}";
            return Productos.GetValueOrDefault(keyProducto, null);
        }

        public virtual bool AnyCrossDocking(int agenda, int preparacion)
        {
            bool activo = false;
            var key = $"{agenda}.{preparacion}";

            var crossDocking = CrossDockingActivos.GetValueOrDefault(key, null);
            if (crossDocking == null)
                activo = true;

            return activo;
        }

        public virtual Agente GetCliente(string tipoAgente, string codigoAgente, int empresa)
        {
            var keyCliente = $"{codigoAgente}.{tipoAgente}.{empresa}";
            return Clientes.GetValueOrDefault(keyCliente, null);
        }

        public virtual Contenedor GetContenedor(string idExternoContenedor, string tipoContenedor)
        {
            return ContenedoresActivos.FirstOrDefault(x => x.IdExterno == idExternoContenedor && x.TipoContenedor == tipoContenedor);
        }

        public virtual DetallePendienteCrossDocking SaldoPendienteXd(int agenda, int preparacion, string cliente, int empresa, string producto, string identificador)
        {
            string key = $"{agenda}.{empresa}.{cliente}.{producto}.{identificador}";
            return DetallesPendienteCrossDocking.GetValueOrDefault(key, null);

        }

        public virtual Stock GetStock(string cdEndereco, string cdProducto, int cdEmpresa, string nuIdentificador, decimal cdFaixa)
        {
            var keyStock = $"{cdEndereco}.{cdProducto}.{cdEmpresa}.{nuIdentificador}.{cdFaixa.ToString("#.###")}";
            return Stocks.FirstOrDefault(x => x.Ubicacion == cdEndereco && x.Producto == cdProducto && x.Empresa == cdEmpresa && x.Identificador == nuIdentificador && x.Faixa == cdFaixa);
        }

        public virtual TipoContenedor GetTipoContenedor(string tpContenedor)
        {
            return TiposContenedor.FirstOrDefault(x => x.Id == tpContenedor);
        }

        public virtual bool EsTipoLpn(string tipoContenedor)
        {
            return TiposLpn.Contains(tipoContenedor);
        }

        public virtual EtiquetaEnUso GetEtiquetaEnUso(string numeroExterno, string tipoEtiqueta)
        {
            return EtiquetasEnUso.FirstOrDefault(x => x.NumeroExterno == numeroExterno && x.TipoEtiqueta == tipoEtiqueta);
        }

        public virtual EtiquetaLoteDetalle GetDetallesEtiquetaLote(int etiquetaLote, string producto, int empresa, decimal faxia, string identificador)
        {
            return EtiquetaLoteDetalle.FirstOrDefault(x => x.IdEtiquetaLote == etiquetaLote && x.CodigoProducto == producto && x.IdEmpresa == empresa && x.Identificador == identificador && x.Faixa == faxia);
        }

        public virtual AgendaDetalle GetDetalle(int numeroAgenda, string producto, int empresa, decimal faixa, string identificador)
        {
            return DetallesAgenda.FirstOrDefault(x => x.IdAgenda == numeroAgenda && x.CodigoProducto == producto
                 && x.IdEmpresa == empresa && x.Faixa == faixa && x.Identificador == identificador);

        }
    }
}
