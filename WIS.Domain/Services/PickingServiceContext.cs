using WIS.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services
{
    public class PickingServiceContext : ServiceContext, IPickingServiceContext
    {
        protected List<DetallePreparacion> _pickeos = new List<DetallePreparacion>();

        public HashSet<Stock> Stocks { get;set;} = new HashSet<Stock>();
        public HashSet<string> Predios { get;set;} = new HashSet<string>();
        public HashSet<string> TiposAgente { get;set;} = new HashSet<string>();
        public HashSet<string> TiposContenedor { get;set;} = new HashSet<string>();
        public HashSet<string> TiposLpn { get;set;} = new HashSet<string>();
        public HashSet<SuperClase> SubClases { get;set;} = new HashSet<SuperClase>();
        public HashSet<CargaCamion> CargasCamion { get;set;} = new HashSet<CargaCamion>();
        public List<DetallePreparacion> DetallesPendientes { get;set;} = new List<DetallePreparacion>();
        public HashSet<PedidoContenedor> PedidosContenedor { get;set;} = new HashSet<PedidoContenedor>();
        public HashSet<DetallePreparacion> DetallesPreparacionDestino { get;set;} = new HashSet<DetallePreparacion>();
        public Dictionary<string, Pedido> Pedidos { get;set;} = new Dictionary<string, Pedido>();
        public Dictionary<string, Agente> Clientes { get;set;} = new Dictionary<string, Agente>();
        public Dictionary<string, Producto> Productos { get;set;} = new Dictionary<string, Producto>();
        public Dictionary<string, Contenedor> Contenedores { get;set;} = new Dictionary<string, Contenedor>();
        public Dictionary<long, short> CargasRutas { get;set;} = new Dictionary<long, short>();
        public Dictionary<int, Carga> PreparacionOrigenCarga { get;set;} = new Dictionary<int, Carga>();
        public Dictionary<int, Carga> PreparacionDescargaCarga { get;set;} = new Dictionary<int, Carga>();
        public Dictionary<short, UbicacionArea> Areas { get;set;} = new Dictionary<short, UbicacionArea>();
        public Dictionary<int, Preparacion> Preparaciones { get;set;} = new Dictionary<int, Preparacion>();
        public Dictionary<string, Ubicacion> Ubicaciones { get;set;} = new Dictionary<string, Ubicacion>();
        public Dictionary<string, UbicacionEquipo> UbicacionesEquipoPredio { get;set;} = new Dictionary<string, UbicacionEquipo>();
        public Dictionary<int, List<DocumentoPreparacionReserva>> ReservasDocumentales { get;set;} = new Dictionary<int, List<DocumentoPreparacionReserva>>();

        public PickingServiceContext(IUnitOfWork uow, List<DetallePreparacion> pickeos, int userId, int empresa) : base(uow, userId, empresa)
        {
            _pickeos = pickeos;
        }

        public async override Task Load()
        {
            var keysCargasCamion = new List<Carga>();
            var keysCargas = new Dictionary<long, Carga>();
            var keysStock = new Dictionary<string, Stock>();
            var keysAgentes = new Dictionary<string, Agente>();
            var keysPedidos = new Dictionary<string, Pedido>();
            var keysProducto = new Dictionary<string, Producto>();
            var keysContenedor = new Dictionary<string, Contenedor>();
            var keysPreparacion = new Dictionary<int, Preparacion>();
            var keysUbicaciones = new Dictionary<string, Ubicacion>();
            var keysDetallesPrep = new Dictionary<string, DetallePreparacion>();

            TiposAgente = _uow.AgenteRepository.GetTiposAgentes().ToHashSet();
            SubClases = _uow.ClaseRepository.GetSuperClases().ToHashSet();
            Areas = _uow.UbicacionAreaRepository.GetUbicacionAreas().ToDictionary(a => a.Id, a => a);
            Predios = _uow.PredioRepository.GetPrediosUsuario(UserId).Select(p => p.Numero).ToHashSet();
            TiposContenedor = _uow.ContenedorRepository.GetTiposContenedores().Select(p => p.Id).ToHashSet();
            TiposLpn = _uow.ManejoLpnRepository.GetTiposLPN().Select(p => p.Tipo).ToHashSet();
            UbicacionesEquipoPredio = _uow.UbicacionRepository.GetUbicacionesEquipo(UserId).ToDictionary(u => u.Predio, u => u);

            foreach (var p in _pickeos)
            {
                var keyProducto = $"{p.Producto}.{Empresa}";
                if (!keysProducto.ContainsKey(keyProducto))
                    keysProducto[keyProducto] = new Producto() { Codigo = p.Producto.Truncate(40), CodigoEmpresa = Empresa };

                var keyAgente = $"{p.TipoAgente}.{p.CodigoAgente}.{Empresa}";
                if (!keysAgentes.ContainsKey(keyAgente))
                    keysAgentes[keyAgente] = new Agente() { Tipo = p.TipoAgente.Truncate(3), Codigo = p.CodigoAgente.Truncate(40), Empresa = Empresa };

                if (!keysPreparacion.ContainsKey(p.NumeroPreparacion))
                    keysPreparacion[p.NumeroPreparacion] = new Preparacion() { Id = p.NumeroPreparacion };

                var keyCont = $"{p.IdExternoContenedor}.{p.TipoContenedor}";
                if (!keysContenedor.ContainsKey(keyCont))
                    keysContenedor[keyCont] = new Contenedor() { IdExterno = p.IdExternoContenedor.Truncate(50), TipoContenedor = p.TipoContenedor.Truncate(10) };

                if (!keysUbicaciones.ContainsKey(p.Ubicacion))
                    keysUbicaciones[p.Ubicacion] = new Ubicacion() { Id = p.Ubicacion.Truncate(40) };

                if (!string.IsNullOrEmpty(p.Contenedor.Ubicacion))
                {
                    if (!keysUbicaciones.ContainsKey(p.Contenedor.Ubicacion))
                        keysUbicaciones[p.Contenedor.Ubicacion] = new Ubicacion() { Id = p.Contenedor.Ubicacion.Truncate(40) };
                }

                var keyStock = $"{p.Ubicacion}.{Empresa}.{p.Producto}.{p.Faixa.ToString("#.###")}.{p.Lote}";
                if (!keysStock.ContainsKey(keyStock))
                    keysStock[keyStock] = new Stock() { Ubicacion = p.Ubicacion.Truncate(40), Empresa = Empresa, Producto = p.Producto.Truncate(40), Faixa = p.Faixa, Identificador = p.Lote.Truncate(40) };

                var keyStockCont = $"{p.Contenedor.Ubicacion}.{Empresa}.{p.Producto}.{p.Faixa.ToString("#.###")}.{p.Lote}";
                if (!keysStock.ContainsKey(keyStockCont))
                    keysStock[keyStockCont] = new Stock() { Ubicacion = p.Contenedor.Ubicacion.Truncate(40), Empresa = Empresa, Producto = p.Producto.Truncate(40), Faixa = p.Faixa, Identificador = p.Lote.Truncate(40) };

                if (p.Carga != null)
                {
                    var c = (long)p.Carga;
                    if (!keysCargas.ContainsKey(c))
                        keysCargas[c] = new Carga() { Id = c };
                }
            }

            Clientes = _uow.AgenteRepository.GetAgentes(keysAgentes.Values).ToDictionary(c => $"{c.Tipo}.{c.Codigo}.{Empresa}", c => c);

            foreach (var p in _pickeos)
            {
                var cliente = GetAgente(p.CodigoAgente, Empresa, p.TipoAgente)?.CodigoInterno;

                var keyPedido = $"{p.Pedido}.{Empresa}.{cliente}";
                if (!keysPedidos.ContainsKey(keyPedido))
                    keysPedidos[keyPedido] = new Pedido() { Id = p.Pedido.Truncate(40), Empresa = Empresa, Cliente = cliente };

                var keyDetalle = $"{p.NumeroPreparacion}.{p.Ubicacion}.{p.Pedido}.{Empresa}.{cliente}.{p.Producto}.{p.Faixa.ToString("#.###")}.{p.Lote}";
                if (!keysDetallesPrep.ContainsKey(keyDetalle))
                    keysDetallesPrep[keyDetalle] = new DetallePreparacion()
                    {
                        NumeroPreparacion = p.NumeroPreparacion,
                        Ubicacion = p.Ubicacion.Truncate(40),
                        Pedido = p.Pedido.Truncate(40),
                        Empresa = Empresa,
                        Cliente = cliente,
                        Producto = p.Producto.Truncate(40),
                        Faixa = p.Faixa,
                        Lote = p.Lote.Truncate(40),
                    };

            }

            DetallesPendientes = _uow.PreparacionRepository.GetDetallesPendientes(keysDetallesPrep.Values).ToList();

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProducto.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);

                var keyProducto = $"{p.Codigo}.{p.CodigoEmpresa}";
                Productos[keyProducto] = p;
            }

            foreach (var c in _uow.ContenedorRepository.GetContenedores(keysContenedor.Values))
            {
                var keyCont = $"{c.IdExterno}.{c.TipoContenedor}";
                Contenedores[keyCont] = c;

                if (!keysPreparacion.ContainsKey(c.NumeroPreparacion))
                    keysPreparacion[c.NumeroPreparacion] = new Preparacion() { Id = c.NumeroPreparacion };

                if (!keysUbicaciones.ContainsKey(c.Ubicacion))
                    keysUbicaciones[c.Ubicacion] = new Ubicacion() { Id = c.Ubicacion.Truncate(40) };
            }

            Preparaciones = _uow.PreparacionRepository.GetPreparaciones(keysPreparacion.Values).ToDictionary(p => p.Id, p => p);

            foreach (var p in _pickeos)
            {
                var predio = GetPreparacion(p.NumeroPreparacion)?.Predio;

                if (!string.IsNullOrEmpty(predio))
                {
                    var ubicacionEquipo = GetUbicacionEquipo(predio);

                    var keyStockEquipo = $"{ubicacionEquipo?.Ubicacion}.{Empresa}.{p.Producto}.{p.Faixa.ToString("#.###")}.{p.Lote}";
                    if (!keysStock.ContainsKey(keyStockEquipo))
                        keysStock[keyStockEquipo] = new Stock() { Ubicacion = ubicacionEquipo?.Ubicacion, Empresa = Empresa, Producto = p.Producto.Truncate(40), Faixa = p.Faixa, Identificador = p.Lote.Truncate(40) };
                }

                foreach (var c in _uow.ContenedorRepository.GetContenedores(keysContenedor.Values))
                {
                    var keyStockContenedor = $"{c.Ubicacion}.{Empresa}.{p.Producto}.{p.Faixa.ToString("#.###")}.{p.Lote}";
                    if (!keysStock.ContainsKey(keyStockContenedor))
                        keysStock[keyStockContenedor] = new Stock() { Ubicacion = c.Ubicacion.Truncate(40), Empresa = Empresa, Producto = p.Producto.Truncate(40), Faixa = p.Faixa, Identificador = p.Lote.Truncate(40) };
                }
            }

            foreach (var u in _uow.UbicacionRepository.GetUbicaciones(keysUbicaciones.Values))
            {
                u.EsUbicacionBaja = (u.IdUbicacionBaja ?? "N") == "S";
                u.EsUbicacionSeparacion = (u.IdUbicacionSeparacion ?? "N") == "S";
                u.NecesitaReabastecer = (u.IdNecesitaReabastecer ?? "N") == "S";

                Ubicaciones[u.Id] = u;
            }

            Preparaciones = _uow.PreparacionRepository.GetPreparaciones(keysPreparacion.Values).ToDictionary(p => p.Id, p => p);
            Pedidos = _uow.PedidoRepository.GetPedidos(keysPedidos.Values).ToDictionary(p => $"{p.Id}.{p.Empresa}.{p.Cliente}", p => p);
            PedidosContenedor = _uow.PedidoRepository.GetPedidosContenedor(Contenedores.Values).ToHashSet();

            PreparacionOrigenCarga = DetallesPendientes.GroupBy(p => p.NumeroPreparacion)
                                    .Select(p => new { NumeroPreparacion = p.Key, Carga = p.Min(p => p.Carga) })
                                    .ToDictionary(d => d.NumeroPreparacion, d => new Carga() { Id = (d.Carga ?? 0) });

            PreparacionDescargaCarga = _uow.PreparacionRepository.GetPreparacionCarga(Contenedores.Values);

            keysCargasCamion.AddRange(PreparacionOrigenCarga.Values);
            keysCargasCamion.AddRange(PreparacionDescargaCarga.Values);
            keysCargasCamion = keysCargasCamion.Distinct().ToList();

            CargasCamion = _uow.CargaCamionRepository.GetCargaCamion(keysCargasCamion).ToHashSet();
            DetallesPreparacionDestino = _uow.PreparacionRepository.GetDetallesPreparacionDestino(Contenedores.Values).ToHashSet();

            foreach (var r in _uow.DocumentoRepository.GetReservasPreparacion(Preparaciones.Values))
            {
                if (!ReservasDocumentales.ContainsKey(r.Preparacion))
                    ReservasDocumentales[r.Preparacion] = new List<DocumentoPreparacionReserva>();

                ReservasDocumentales[r.Preparacion].Add(r);
            }

            Stocks = _uow.StockRepository.GetStocks(keysStock.Values).ToHashSet();

            CargasRutas = _uow.CargaRepository.GetCargas(keysCargas.Values).ToDictionary(d => d.Id, d => (short)d.Ruta);

            await base.Load();
        }

        #region Metodos
        public virtual bool ExisteUbicacion(string key)
        {
            return Ubicaciones.TryGetValue(key, out Ubicacion u);
        }

        public virtual bool ExisteTipoAgente(string tipoAgente)
        {
            return TiposAgente.Contains(tipoAgente);
        }

        public virtual bool ExisteTipoContenedor(string tipoContenedor)
        {
            return TiposContenedor.Contains(tipoContenedor);
        }

        public virtual bool EsTipoLpn(string tipoContenedor)
        {
            return TiposLpn.Contains(tipoContenedor);
        }

        public virtual bool ExistePedido(string pedido, int empresa, string cliente)
        {
            var key = $"{pedido}.{empresa}.{cliente}";
            return Pedidos.TryGetValue(key, out Pedido ped);
        }

        public virtual bool ExisteCarga(long nroCarga)
        {
            return CargasRutas.TryGetValue(nroCarga, out short c);
        }

        public virtual bool ExisteRutaDistintaContenedor(int preparacion, int contenedor, int ruta)
        {
            return PedidosContenedor.Any(p => p.Contenedor == contenedor && p.Preparacion == preparacion && p.Ruta != ruta);
        }

        public virtual bool ExisteClienteDistintoContenedor(int preparacion, int contenedor, string cliente)
        {
            return DetallesPreparacionDestino.Any(p => p.NumeroPreparacion == preparacion && p.NroContenedor == contenedor && p.Cliente != cliente);
        }

        public virtual bool PuedeCompartirContenedorPicking(string cliente, int contenedor, string valorCompatibilidad)
        {
            if (PedidosContenedor == null || PedidosContenedor.Count == 0)
                return true;

            return PedidosContenedor
                   .Any(p => p.Contenedor == contenedor
                   && string.IsNullOrEmpty(cliente) ? true : p.Cliente == cliente
                   && p.EstadoContenedor == SituacionDb.ContenedorEnPreparacion
                   && ((string.IsNullOrEmpty(p.ComparteContenedorPicking) && string.IsNullOrEmpty(valorCompatibilidad))
                        || p.ComparteContenedorPicking == valorCompatibilidad));
        }

        public virtual bool ManejaDocumental()
        {
            return (GetParametro(ParamManager.MANEJO_DOCUMENTAL) ?? "N") == "S";
        }

        public virtual Contenedor GetContenedor(string idExterno, string tipoContenedor)
        {
            var keyCont = $"{idExterno}.{tipoContenedor}";
            return Contenedores.GetValueOrDefault(keyCont, null);
        }

        public virtual CargaCamion GetCargaCamion(long? carga)
        {
            return CargasCamion.FirstOrDefault(c => c.Carga == carga);
        }

        public virtual UbicacionEquipo GetUbicacionEquipo(string predio)
        {
            return UbicacionesEquipoPredio.GetValueOrDefault(predio, null);
        }

        public virtual Ubicacion GetUbicacion(string ubicacion)
        {
            return Ubicaciones.GetValueOrDefault(ubicacion, null);
        }

        public virtual Preparacion GetPreparacion(int preparacion)
        {
            return Preparaciones.GetValueOrDefault(preparacion, null);
        }

        public virtual string GetAgrupacion(int preparacion)
        {
            return GetPreparacion(preparacion).Agrupacion;
        }

        public virtual SuperClase GetSubClaseProducto(string clase)
        {
            return SubClases.Where(x => x.Clases.Any(y => y.Id == clase)).FirstOrDefault();
        }

        public virtual long? GetNroCarga(int preparacion, bool origen)
        {
            if (origen)
                return PreparacionOrigenCarga.GetValueOrDefault(preparacion, null)?.Id;
            else
                return PreparacionDescargaCarga.GetValueOrDefault(preparacion, null)?.Id;
        }

        public virtual Producto GetProducto(string codigo, int empresa)
        {
            var key = $"{codigo}.{empresa}";
            return Productos.GetValueOrDefault(key, null);
        }

        public virtual Agente GetAgente(string codigo, int empresa, string tipo)
        {
            var key = $"{tipo}.{codigo}.{empresa}";
            return Clientes.GetValueOrDefault(key, null);
        }

        public virtual Pedido GetPedido(string pedido, int empresa, string cliente)
        {
            var key = $"{pedido}.{empresa}.{cliente}";
            return Pedidos.GetValueOrDefault(key, null);
        }

        public virtual short GetRuta(long nroCarga)
        {
            return CargasRutas.GetValueOrDefault(nroCarga, default);
        }

        public virtual IEnumerable<DetallePreparacion> GetDetallesPendiente(DetallePreparacion pick, string agrupacion)
        {
            switch (agrupacion)
            {
                case Agrupacion.Pedido:

                    return DetallesPendientes
                    .Where(d => d.NumeroPreparacion == pick.NumeroPreparacion
                            && d.Ubicacion == pick.Ubicacion
                            && d.Pedido == pick.Pedido
                            && d.Empresa == pick.Empresa
                            && d.Cliente == pick.Cliente
                            && d.Producto == pick.Producto
                            && d.Lote == pick.Lote
                            && d.Faixa == pick.Faixa
                            && d.Estado == pick.Estado);

                case Agrupacion.Cliente:

                    return DetallesPendientes
                   .Where(d => d.NumeroPreparacion == pick.NumeroPreparacion
                       && d.Ubicacion == pick.Ubicacion
                       && d.Empresa == pick.Empresa
                       && d.Cliente == pick.Cliente
                       && d.Producto == pick.Producto
                       && d.Lote == pick.Lote
                       && d.Faixa == pick.Faixa
                       && d.Estado == pick.Estado
                       && ((string.IsNullOrEmpty(d.ComparteContenedorPicking) && string.IsNullOrEmpty(pick.ComparteContenedorPicking))
                           || d.ComparteContenedorPicking == pick.ComparteContenedorPicking));

                case Agrupacion.Ruta:

                    return DetallesPendientes
                     .Where(d => d.NumeroPreparacion == pick.NumeroPreparacion
                         && d.Ubicacion == pick.Ubicacion
                         && d.Empresa == pick.Empresa
                         && d.Producto == pick.Producto
                         && d.Lote == pick.Lote
                         && d.Faixa == pick.Faixa
                         && d.Estado == pick.Estado
                         && d.Carga == pick.Carga
                         && ((string.IsNullOrEmpty(d.ComparteContenedorPicking) && string.IsNullOrEmpty(pick.ComparteContenedorPicking))
                             || d.ComparteContenedorPicking == pick.ComparteContenedorPicking));

                case Agrupacion.Onda:

                    return DetallesPendientes
                   .Where(d => d.NumeroPreparacion == pick.NumeroPreparacion
                       && d.Ubicacion == pick.Ubicacion
                       && d.Empresa == pick.Empresa
                       && d.Producto == pick.Producto
                       && d.Lote == pick.Lote
                       && d.Faixa == pick.Faixa
                       && d.Estado == pick.Estado
                       && ((string.IsNullOrEmpty(d.ComparteContenedorPicking) && string.IsNullOrEmpty(pick.ComparteContenedorPicking))
                           || d.ComparteContenedorPicking == pick.ComparteContenedorPicking));

                default:
                    return null;
            }
        }

        public virtual decimal GetCantidadPendiente(DetallePreparacion pick, string agrupacion)
        {
            return GetDetallesPendiente(pick, agrupacion).Sum(d => d.Cantidad);
        }

        public virtual IEnumerable<DocumentoPreparacionReserva> GetReservasDetalles(int preparacion, int empresa, string producto, string identificador, decimal faixa)
        {
            var reservas = ReservasDocumentales.GetValueOrDefault(preparacion, new List<DocumentoPreparacionReserva>());
            return reservas.Where(d => d.Preparacion == preparacion
            && d.Empresa == empresa
            && d.Producto == producto
            && d.Identificador == identificador
            && d.Faixa == faixa);
        }

        public virtual DocumentoPreparacionReserva GetReservasDocumento(string nroDocumento, string tpDocumento, int preparacion, int empresa, string producto, string identificador, decimal faixa)
        {
            var reservas = ReservasDocumentales.GetValueOrDefault(preparacion, new List<DocumentoPreparacionReserva>());
            return reservas.FirstOrDefault(d => d.Preparacion == preparacion
            && d.Empresa == empresa
            && d.Producto == producto
            && d.Identificador == identificador
            && d.Faixa == faixa
            && d.NroDocumento == nroDocumento
            && d.TipoDocumento == tpDocumento);
        }

        public virtual Stock GetStock(string ubicacion, int empresa, string producto, string identificador, decimal faixa)
        {
            return Stocks.FirstOrDefault(s => s.Ubicacion == ubicacion && s.Producto == producto && s.Empresa == empresa && s.Identificador == identificador && s.Faixa == faixa);
        }
        #endregion
    }
}
