using WIS.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class EgresoServiceContext : ServiceContext, IEgresoServiceContext
    {
        protected List<Camion> _egresos = new List<Camion>();

        public KeyDetalles KeysDetallesEgreso { get; set; } = new KeyDetalles();
        public List<Predio> Predios { get; set; } = new List<Predio>();
        public HashSet<int> Empresas { get; set; } = new HashSet<int>();
        public List<PuertaEmbarque> Puertas { get; set; } = new List<PuertaEmbarque>();
        public HashSet<int> Transportistas { get; set; } = new HashSet<int>();
        public HashSet<string> NuPredios { get; set; } = new HashSet<string>();
        public HashSet<string> IdsExternos { get; set; } = new HashSet<string>();
        public HashSet<string> NuPrediosExternos { get; set; } = new HashSet<string>();
        public HashSet<string> CargasEnOtroCamion { get; set; } = new HashSet<string>();
        public HashSet<Carga> CargasCompartidasPedidos { get; set; } = new HashSet<Carga>();
        public HashSet<Carga> CargasCompartidasContenedores { get; set; } = new HashSet<Carga>();
        public Dictionary<short, Ruta> Rutas { get; set; } = new Dictionary<short, Ruta>();
        public Dictionary<string, Agente> Agentes { get; set; } = new Dictionary<string, Agente>();
        public Dictionary<int, Vehiculo> Vehiculos { get; set; } = new Dictionary<int, Vehiculo>();
        public Dictionary<long, Carga> CargasHabilitadas { get; set; } = new Dictionary<long, Carga>();
        public Dictionary<string, Pedido> PedidosHabilitados { get; set; } = new Dictionary<string, Pedido>();
        public Dictionary<string, Pedido> PedidosConPendientes { get; set; } = new Dictionary<string, Pedido>();
        public Dictionary<string, Contenedor> ContenedoresHabilitados { get; set; } = new Dictionary<string, Contenedor>();
        public Dictionary<string, List<DetallePreparacion>> PedidosCargasLiberadas { get; set; } = new Dictionary<string, List<DetallePreparacion>>();
        public Dictionary<string, List<ContenedorExternoCarga>> ContenedoresCargasLiberadas { get; set; } = new Dictionary<string, List<ContenedorExternoCarga>>();

        public EgresoServiceContext(IUnitOfWork uow, List<Camion> egresos, int userId, int empresaEjecucion) : base(uow, userId, empresaEjecucion)
        {
            _egresos = egresos;
            KeysDetallesEgreso = new KeyDetalles();
        }

        public async override Task Load()
        {
            await base.Load();

            var keysCargas = new Dictionary<long, Carga>();
            var keysPedidos = new Dictionary<string, Pedido>();
            var keysContenedores = new Dictionary<string, Contenedor>();
            var keysParcialCargaCamion = new Dictionary<string, CargaCamion>();

            Predios = _uow.PredioRepository.GetPrediosUsuario(UserId);
            NuPredios = Predios.Select(p => p.Numero).ToHashSet();
            NuPrediosExternos = Predios.Where(p => p.IdExterno != null).Select(p => p.IdExterno).ToHashSet();

            IdsExternos = _uow.CamionRepository.GetIdentificadoresExternos();
            Rutas = _uow.RutaRepository.GetRutas().ToDictionary(r => r.Id, r => r);
            Puertas = _uow.PuertaEmbarqueRepository.GetPuertasPredio(NuPredios);
            Empresas = _uow.EmpresaRepository.GetEmpresasAsignadasUsuario(UserId).ToHashSet();
            Vehiculos = _uow.VehiculoRepository.GetVehiculos().ToDictionary(v => v.Id, v => v);
            Transportistas = _uow.TransportistaRepository.GetTransportistas().Select(p => p.Id).ToHashSet();

            //No cambiar el orden de los siguientes metodos.
            ProcesoAgentes();
            ProcesoClaves(keysCargas, keysPedidos, keysContenedores, keysParcialCargaCamion, out bool validacionCargaPikcing);
            ProcesoPedidos(keysPedidos, keysParcialCargaCamion);
            ProcesoCargas(keysCargas, validacionCargaPikcing);
            ProcesoContenedores(keysContenedores, keysParcialCargaCamion);
            ProcesoCargasCamion(keysParcialCargaCamion);
        }

        #region Metodos auxiliares

        public virtual bool ExistePuerta(short puerta)
        {
            return Puertas.Any(x => x.Id == puerta);
        }
        public virtual PuertaEmbarque GetPuerta(short puerta)
        {
            return Puertas.FirstOrDefault(x => x.Id == puerta);
        }
        public virtual bool CargaEnOtroCamion(string cargaCliente)
        {
            return CargasEnOtroCamion.Contains(cargaCliente);
        }
        public virtual bool ExistePredio(string predio)
        {
            return NuPredios.Contains(predio);
        }
        public virtual bool ExistePredioExterno(string idExterno)
        {
            return NuPrediosExternos.Contains(idExterno);
        }
        public virtual bool ExisteEmpresa(int cdEmpresa)
        {
            return Empresas.Contains(cdEmpresa);
        }
        public virtual bool ExisteIdExterno(string idExterno)
        {
            var key = $"{idExterno}.{Empresa}";
            return IdsExternos.Contains(key);
        }
        public virtual bool ExisteTransportista(int transportadora)
        {
            return Transportistas.Contains(transportadora);
        }

        public virtual Ruta GetRuta(short cdRuta)
        {
            return Rutas.GetValueOrDefault(cdRuta, null);
        }
        public virtual Vehiculo GetVehiculo(int cdVehiculo)
        {
            return Vehiculos.GetValueOrDefault(cdVehiculo, null);
        }
        public virtual Carga GetCargaHabilitada(long nuCarga)
        {
            return CargasHabilitadas.GetValueOrDefault(nuCarga, null);
        }
        public virtual Agente GetAgente(string codigo, int empresa, string tipo)
        {
            return Agentes.GetValueOrDefault($"{codigo}.{tipo}.{empresa}", null);
        }
        public virtual Contenedor GetContenedorHabilitado(string idExternoContenedor, string tipoContenedor, int empresa)
        {
            var key = $"{idExternoContenedor}.{tipoContenedor}.{empresa}";
            return ContenedoresHabilitados.GetValueOrDefault(key, null);
        }
        public virtual Pedido GetPedidoHabilitado(string pedido, int empresa, string cliente)
        {
            var key = $"{pedido}.{empresa}.{cliente}";
            return PedidosHabilitados.GetValueOrDefault(key, null);
        }

        public virtual void ProcesoAgentes()
        {
            //Cargo key de agentes
            var keysAgentes = new Dictionary<string, Agente>();
            foreach (var e in _egresos)
            {
                foreach (var p in e.DetalleArmadoEgreso.Pedidos)
                {
                    var keyAgente = $"{p.CodigoAgente}.{p.TipoAgente}.{p.Empresa}";
                    if (!keysAgentes.ContainsKey(keyAgente))
                        keysAgentes[keyAgente] = new Agente() { Codigo = p.CodigoAgente.Truncate(40), Tipo = p.TipoAgente.Truncate(3), Empresa = p.Empresa };
                }

                foreach (var ca in e.DetalleArmadoEgreso.Cargas)
                {
                    var keyAgente = $"{ca.CodigoAgente}.{ca.TipoAgente}.{ca.Empresa}";
                    if (!keysAgentes.ContainsKey(keyAgente))
                        keysAgentes[keyAgente] = new Agente() { Codigo = ca.CodigoAgente.Truncate(40), Tipo = ca.TipoAgente.Truncate(3), Empresa = ca.Empresa };
                }
            }

            //Obtengo los agentes en base a las keys otorgadas
            var agentes = _uow.AgenteRepository.GetAgentes(keysAgentes.Values);
            foreach (var a in agentes)
            {
                var key = $"{a.Codigo}.{a.Tipo}.{a.Empresa}";
                Agentes[key] = a;
            }
        }
        public virtual void ProcesoClaves(Dictionary<long, Carga> keysCargas, Dictionary<string, Pedido> keysPedidos, Dictionary<string, Contenedor> keysContenedores, Dictionary<string, CargaCamion> keysParcialCargaCamion, out bool validacionCargaPicking)
        {
            foreach (var e in _egresos)
            {
                foreach (var p in e.DetalleArmadoEgreso.Pedidos)
                {
                    var keyAgente = $"{p.CodigoAgente}.{p.TipoAgente}.{p.Empresa}";
                    var agente = Agentes.GetValueOrDefault(keyAgente, null);

                    //Cargo la key del pedido con CD_CLIENTE, por este motivo tuve que traer los agentes previamente
                    var keyPedido = $"{p.NroPedido}.{p.Empresa}.{agente?.CodigoInterno}";
                    keysPedidos[keyPedido] = new Pedido()
                    {
                        Id = p.NroPedido,
                        Empresa = p.Empresa,
                        Cliente = agente?.CodigoInterno
                    };
                }

                foreach (var ca in e.DetalleArmadoEgreso.Cargas)
                {
                    //Cargo key de la carga enviada
                    if (!keysCargas.ContainsKey(ca.Carga))
                        keysCargas[ca.Carga] = new Carga() { Id = ca.Carga };

                    var keyAgente = $"{ca.CodigoAgente}.{ca.TipoAgente}.{ca.Empresa}";
                    var agente = Agentes.GetValueOrDefault(keyAgente, null);

                    var keyParcialCargaCamion = $"{ca.Carga}.{agente?.CodigoInterno}.{ca.Empresa}";
                    if (!keysParcialCargaCamion.ContainsKey(keyParcialCargaCamion))
                        keysParcialCargaCamion[keyParcialCargaCamion] = new CargaCamion() { Carga = ca.Carga, Cliente = agente?.CodigoInterno, Empresa = ca.Empresa };
                }

                foreach (var co in e.DetalleArmadoEgreso.Contenedores)
                {
                    //Cargo key de la carga enviada
                    var keyContenedor = $"{co.IdExternoContenedor}.{co.TipoContenedor}.{co.Empresa}";
                    if (!keysContenedores.ContainsKey(keyContenedor))
                        keysContenedores[keyContenedor] = new Contenedor() { IdExterno = co.IdExternoContenedor, TipoContenedor = co.TipoContenedor, Empresa = co.Empresa };
                }
            }

            validacionCargaPicking = !_egresos.Any(c => c.SincronizacionRealizadaId == "S");
        }
        public virtual void ProcesoPedidos(Dictionary<string, Pedido> keysPedidos, Dictionary<string, CargaCamion> keysParcialCargaCamion)
        {
            if (keysPedidos.Count > 0)
            {
                //Obtengo los pedidos habilitados en base a las keys otorgadas
                var pedidos = _uow.CargaRepository.GetPedidosHabilitados(keysPedidos.Values);
                foreach (var p in pedidos)
                {
                    var key = $"{p.Id}.{p.Empresa}.{p.Cliente}";
                    PedidosHabilitados[key] = p;
                }

                PedidosConPendientes = _uow.CargaRepository.GetPedidosConPendientes(PedidosHabilitados.Values);

                var detsPickingPedido = _uow.CargaRepository.GetDetsPreparacionPedido(PedidosHabilitados.Values);
                foreach (var dp in detsPickingPedido)
                {
                    var keyPedido = $"{dp.Pedido}.{dp.Empresa}.{dp.Cliente}";
                    if (!PedidosCargasLiberadas.ContainsKey(keyPedido))
                        PedidosCargasLiberadas[keyPedido] = new List<DetallePreparacion>();

                    PedidosCargasLiberadas[keyPedido].Add(dp);

                    var keyParcialCargaCamion = $"{dp.Carga}.{dp.Cliente}.{dp.Empresa}";
                    if (!keysParcialCargaCamion.ContainsKey(keyParcialCargaCamion))
                        keysParcialCargaCamion[keyParcialCargaCamion] = new CargaCamion() { Carga = (dp.Carga ?? -1), Cliente = dp.Cliente, Empresa = dp.Empresa };
                }
            }
        }
        public virtual void ProcesoCargas(Dictionary<long, Carga> keysCargas, bool validacionPicking)
        {
            if (keysCargas.Count > 0)
            {
                //Obtengo las cargas habilitadas en base a las keys otorgadas
                var cargas = _uow.CargaRepository.GetCargasHabilitadas(keysCargas.Values, validacionPicking);
                foreach (var ca in cargas)
                {
                    CargasHabilitadas[ca.Id] = ca;
                }
            }
        }
        public virtual void ProcesoContenedores(Dictionary<string, Contenedor> keysContenedores, Dictionary<string, CargaCamion> keysParcialCargaCamion)
        {
            if (keysContenedores.Count > 0)
            {
                //Obtengo los contenedores habilitados en base a las keys otorgadas
                var contenedores = _uow.CargaRepository.GetContenedoresHabilitados(keysContenedores.Values);
                foreach (var co in contenedores)
                {
                    var keyContenedor = $"{co.IdExterno}.{co.TipoContenedor}.{co.Empresa}";
                    ContenedoresHabilitados[keyContenedor] = co;
                }

                var cargasContenedor = _uow.CargaRepository.GetCargasContenedores(ContenedoresHabilitados.Values);
                foreach (var co in cargasContenedor)
                {
                    var keyContenedor = $"{co.IdExternoContenedor}.{co.TipoContenedor}.{co.Empresa}";
                    if (!ContenedoresCargasLiberadas.ContainsKey(keyContenedor))
                        ContenedoresCargasLiberadas[keyContenedor] = new List<ContenedorExternoCarga>();

                    ContenedoresCargasLiberadas[keyContenedor].Add(co);

                    var keyParcialCargaCamion = $"{co.Carga}.{co.Cliente}.{co.Empresa}";
                    if (!keysParcialCargaCamion.ContainsKey(keyParcialCargaCamion))
                        keysParcialCargaCamion[keyParcialCargaCamion] = new CargaCamion() { Carga = co.Carga, Cliente = co.Cliente, Empresa = co.Empresa };
                }
            }
        }
        public virtual void ProcesoCargasCamion(Dictionary<string, CargaCamion> keysParcialCargaCamion)
        {
            if (keysParcialCargaCamion.Count > 0)
            {
                var cargasClientesCamion = _uow.CargaCamionRepository.GetCargasCamion(keysParcialCargaCamion.Values);
                foreach (var ccc in cargasClientesCamion)
                {
                    var key = $"{ccc.Carga}.{ccc.Cliente}.{ccc.Empresa}";
                    CargasEnOtroCamion.Add(key);
                }

                CargasCompartidasPedidos = _uow.CargaCamionRepository.GetCargasCompartidasPedidos(keysParcialCargaCamion.Values).ToHashSet();
                CargasCompartidasContenedores = _uow.CargaCamionRepository.GetCargasCompartidasContenedores(keysParcialCargaCamion.Values).ToHashSet();
            }
        }
        #endregion

        public class KeyDetalles
        {
            public HashSet<string> KeyPedidos { get; set; }
            public HashSet<string> KeyCargas { get; set; }
            public HashSet<string> KeyContenedores { get; set; }

            public KeyDetalles()
            {
                KeyPedidos = new HashSet<string>();
                KeyCargas = new HashSet<string>();
                KeyContenedores = new HashSet<string>();
            }
        }

    }
}
