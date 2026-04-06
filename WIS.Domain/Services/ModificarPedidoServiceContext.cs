using WIS.Extension;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services
{
    public class ModificarPedidoServiceContext : ServiceContext, IModificarPedidoServiceContext
    {
        protected List<Pedido> _pedidos = new List<Pedido>();
        protected Dictionary<string, Lpn> _lpns = new Dictionary<string, Lpn>();
        protected Dictionary<long, List<LpnDetalle>> _detallesLpn = new Dictionary<long, List<LpnDetalle>>();
        protected Dictionary<string, LpnTipo> _tiposLpn = new Dictionary<string, LpnTipo>();

        public HashSet<short> Rutas { get; set; } = new HashSet<short>();
        public HashSet<string> Predios { get; set; } = new HashSet<string>();
        public HashSet<int> Transportadoras { get; set; } = new HashSet<int>();
        public HashSet<string> TiposAgente { get; set; } = new HashSet<string>();
        public HashSet<string> TiposPedido { get; set; } = new HashSet<string>();
        public HashSet<string> TiposExpedicion { get; set; } = new HashSet<string>();
        public HashSet<string> TiposExpedicionPedido { get; set; } = new HashSet<string>();
        public HashSet<string> CondicionesLiberacion { get; set; } = new HashSet<string>();
        public HashSet<string> DetallePedidoLpnAtributos { get; set; } = new HashSet<string>();
        public HashSet<string> Zonas { get; set; } = new HashSet<string>();

        public Dictionary<string, string> Agentes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, Pedido> Pedidos { get; set; } = new Dictionary<string, Pedido>();
        public Dictionary<string, Producto> Productos { get; set; } = new Dictionary<string, Producto>();
        public Dictionary<string, decimal?> SaldosTotales { get; set; } = new Dictionary<string, decimal?>();
        public Dictionary<string, bool> EditarTipoHabilitado { get; set; } = new Dictionary<string, bool>();

        //Contiene todos los detalles del pedido
        public Dictionary<string, DetallePedido> Detalles { get; set; } = new Dictionary<string, DetallePedido>();
        //Contiene todos los duplicados del pedido
        public Dictionary<string, DetallePedidoDuplicado> Duplicados { get; set; } = new Dictionary<string, DetallePedidoDuplicado>();

        public Dictionary<string, Atributo> Atributos { get; set; } = new Dictionary<string, Atributo>();
        public Dictionary<string, List<DetallePedidoAtributos>> DetallePedidoAtributos { get; set; } = new Dictionary<string, List<DetallePedidoAtributos>>();

        public Dictionary<string, DetallePedidoLpn> DetallesPedidoLpn { get; set; } = new Dictionary<string, DetallePedidoLpn>();
        public Dictionary<string, List<DetallePedidoLpn>> PedidoDetallesLpn { get; set; } = new Dictionary<string, List<DetallePedidoLpn>>();
        public Dictionary<string, List<DetallePedidoAtributosLpn>> DetallePedidoAtributosLpn { get; set; } = new Dictionary<string, List<DetallePedidoAtributosLpn>>();

        // Dada la key de un detalle AUTO, retorna las keys de los detalles NO AUTO asociados al mismo
        public Dictionary<string, List<string>> DetallesConLoteAsociados { get; set; } = new Dictionary<string, List<string>>();
        //Dada la key de un detalle, retorna sus duplicados. En el caso de detalles AUTO, solo retorna duplicados AUTO. Para los detalles NO AUTO, retorna los duplicados NO AUTO.
        public Dictionary<string, List<string>> DuplicadosAsociados { get; set; } = new Dictionary<string, List<string>>();
        // Dada la key de un duplicado AUTO, retorna las keys de los duplicados NO AUTO asociados al mismo.
        public Dictionary<string, List<string>> DuplicadosConLoteAsociados { get; set; } = new Dictionary<string, List<string>>();

        public ModificarPedidoServiceContext(IUnitOfWork uow, List<Pedido> pedidos, int userId, int empresa) : base(uow, userId, empresa)
        {
            _pedidos = pedidos;
        }

        public async override Task Load()
        {
            await base.Load();

            var keysAgentes = new Dictionary<string, Agente>();
            var keysProductos = new Dictionary<string, Producto>();
            var keysRutas = new Dictionary<string, Ruta>();
            var keysDetalleParcial = new Dictionary<string, object>();

            var keysLpn = new Dictionary<string, Lpn>();
            var keysTipoLpn = new Dictionary<string, LpnTipo>();
            var keysDetallesLpn = new Dictionary<string, LpnDetalle>();

            var detallesLpn = new Dictionary<string, LpnDetalle>();
            var keysAtributos = new Dictionary<string, Atributo>();
            var keysDetalleLpnAtributos = new Dictionary<string, LpnDetalleAtributo>();

            foreach (var ta in _uow.AgenteRepository.GetTiposAgentes())
            {
                TiposAgente.Add(ta);
            }

            foreach (var tp in _uow.PedidoRepository.GetTiposPedido().Keys)
            {
                TiposPedido.Add(tp);
            }

            foreach (var te in _uow.PedidoRepository.GetConfiguracionesExpedicion())
            {
                TiposExpedicion.Add(te.Tipo);
            }

            foreach (var t in _uow.PedidoRepository.GetTiposExpedicionPedido())
            {
                TiposExpedicionPedido.Add($"{t.Item1}.{t.Item2}");
            }

            foreach (var p in _uow.PredioRepository.GetPrediosUsuario(UserId))
            {
                Predios.Add(p.Numero);
            }

            foreach (var cl in _uow.LiberacionRepository.GetCondicionLiberaciones())
            {
                CondicionesLiberacion.Add(cl.Condicion);
            }

            foreach (var t in _uow.TransportistaRepository.GetTransportistas())
            {
                Transportadoras.Add(t.Id);
            }

            foreach (var p in _pedidos)
            {
                if (p.Ruta.HasValue)
                {
                    var keyRuta = $"{p.Ruta.Value}";
                    keysRutas[keyRuta] = new Ruta() { Id = (short)p.Ruta.Value };
                }

                var keyAgente = $"{p.TipoAgente}.{p.CodigoAgente}.{Empresa}";
                keysAgentes[keyAgente] = new Agente() { Tipo = p.TipoAgente.Truncate(3), Codigo = p.CodigoAgente.Truncate(40), Empresa = Empresa };

                foreach (var l in p.Lpns)
                {
                    var keyLpn = $"{l.IdExterno}.{l.Tipo}.{Empresa}";
                    keysLpn[keyLpn] = new Lpn() { IdExterno = l.IdExterno.Truncate(50), Tipo = l.Tipo.Truncate(10), Empresa = Empresa };
                    var keyTipo = $"{l.Tipo}";
                    keysTipoLpn[keyTipo] = new LpnTipo() { Tipo = l.Tipo.Truncate(10) };
                }

                foreach (var d in _uow.ManejoLpnRepository.GetDetallesLpn(keysLpn.Values))
                {
                    var keyDetalleLpn = $"{d.Id}.{d.NumeroLPN}.{d.CodigoProducto}.{d.Faixa.ToString("#.###")}.{d.Empresa}.{d.Lote}";
                    detallesLpn[keyDetalleLpn] = d;
                }

                foreach (var l in p.Lineas)
                {
                    var keyProducto = $"{l.Producto}.{Empresa}";
                    keysProductos[keyProducto] = new Producto() { Codigo = l.Producto.Truncate(40), CodigoEmpresa = Empresa };

                    foreach (var detalleLpn in l.DetallesLpn)
                    {
                        var keyLpn = $"{detalleLpn.IdLpnExterno}.{detalleLpn.Tipo}.{Empresa}";
                        keysLpn[keyLpn] = new Lpn() { IdExterno = detalleLpn.IdLpnExterno.Truncate(50), Tipo = detalleLpn.Tipo.Truncate(10), Empresa = Empresa };
                        var keyTipo = $"{detalleLpn.Tipo}";
                        keysTipoLpn[keyTipo] = new LpnTipo() { Tipo = detalleLpn.Tipo.Truncate(10) };

                        var keyDetallesLpn = $"{detalleLpn.IdLpnExterno}.{detalleLpn.Tipo}";
                        keysDetallesLpn[keyDetallesLpn] = new LpnDetalle()
                        {
                            IdExterno = detalleLpn.IdLpnExterno.Truncate(50),
                            CodigoProducto = l.Producto.Truncate(40),
                            Tipo = detalleLpn.Tipo.Truncate(10),
                            Empresa = Empresa,
                            Lote = detalleLpn.Identificador.Truncate(40)
                        };

                        foreach (var configuracion in detalleLpn.Atributos)
                        {
                            foreach (var atributo in configuracion.Atributos)
                            {
                                var keyDetalleLpnAtributo = $"{keyDetallesLpn}.{detalleLpn.Empresa}.{detalleLpn.Producto}.{detalleLpn.Faixa.ToString("#.###")}.{detalleLpn.Identificador}.{atributo.Nombre}";

                                keysAtributos[atributo.Nombre] = new Atributo
                                {
                                    Nombre = atributo.Nombre.Truncate(50),
                                };

                                keysDetalleLpnAtributos[keyDetalleLpnAtributo] = new LpnDetalleAtributo
                                {
                                    IdLpnExterno = detalleLpn.IdLpnExterno.Truncate(50),
                                    Tipo = detalleLpn.Tipo.Truncate(10),
                                    Empresa = Empresa,
                                    Producto = detalleLpn.Producto.Truncate(40),
                                    Faixa = detalleLpn.Faixa,
                                    Lote = detalleLpn.Identificador.Truncate(40),
                                    NombreAtributo = atributo.Nombre.Truncate(50),
                                };
                            }
                        }
                    }

                    foreach (var configuracion in l.Atributos)
                    {
                        foreach (var atributo in configuracion.Atributos)
                        {
                            keysAtributos[atributo.Nombre] = new Atributo
                            {
                                Nombre = atributo.Nombre.Truncate(50),
                            };
                        }
                    }
                }
            }

            foreach (var l in _uow.ManejoLpnRepository.GetLpnsActivos(keysLpn.Values))
            {
                _lpns[$"{l.Tipo}.{l.IdExterno}"] = l;
            }

            foreach (var d in _uow.ManejoLpnRepository.GetDetallesLpnActivo(keysDetallesLpn.Values))
            {
                var keyDetalleLpn = $"{d.Id}.{d.NumeroLPN}.{d.CodigoProducto}.{d.Faixa.ToString("#.###")}.{d.Empresa}.{d.Lote}";
                detallesLpn[keyDetalleLpn] = d;
            }

            foreach (var d in _uow.ManejoLpnRepository.GetAtributosDetallesLpn(keysDetalleLpnAtributos.Values))
            {
                var keyDetalleLpnAtributo = $"{d.IdLpnExterno}.{d.Tipo}.{d.Empresa}.{d.Producto}.{d.Faixa.ToString("#.###")}.{d.Lote}.{d.NombreAtributo}";

                if (!DetallePedidoLpnAtributos.Contains(keyDetalleLpnAtributo))
                    DetallePedidoLpnAtributos.Add(keyDetalleLpnAtributo);
            }

            foreach (var d in detallesLpn.Values)
            {
                if (!_detallesLpn.ContainsKey(d.NumeroLPN))
                    _detallesLpn[d.NumeroLPN] = new List<LpnDetalle>();

                _detallesLpn[d.NumeroLPN].Add(d);
            }

            foreach (var t in _uow.ManejoLpnRepository.GetTiposLpn(keysTipoLpn.Values))
            {
                _tiposLpn[t.Tipo] = t;
            }

            var rutaParam = GetParametro(ParamManager.IE_503_CD_ROTA);
            if (short.TryParse(rutaParam, out short ruta) && !keysRutas.ContainsKey(rutaParam))
                keysRutas.Add(rutaParam, new Ruta() { Id = ruta });

            foreach (var r in _uow.RutaRepository.GetRutas(keysRutas.Values))
            {
                Rutas.Add(r);
            }

            foreach (var a in _uow.AgenteRepository.GetAgentes(keysAgentes.Values))
            {
                Agentes[($"{a.Tipo}.{a.Codigo}")] = a.CodigoInterno;
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProductos.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);
                Productos[p.Codigo] = p;
            }

            foreach (var p in _pedidos)
            {
                var cliente = Agentes.GetValueOrDefault($"{p.TipoAgente}.{p.CodigoAgente}", string.Empty);
                if (!string.IsNullOrEmpty(cliente))
                    p.Cliente = cliente;

                if (p.Lineas != null && p.Lineas.Count > 0)
                {
                    foreach (var det in p.Lineas)
                    {
                        det.Cliente = p.Cliente;
                        det.Identificador = GetIdentificador(det.Identificador, det.Producto);

                        if (det.Identificador == ManejoIdentificadorDb.IdentificadorProducto)
                            det.EspecificaIdentificadorId = "S";

                        var keyParcial = $"{p.Id}.{p.Cliente}.{Empresa}.{det.Producto}.{det.EspecificaIdentificadorId}";
                        if (!keysDetalleParcial.ContainsKey(keyParcial))
                            keysDetalleParcial[keyParcial] = new { Id = p.Id, Cliente = p.Cliente.Truncate(10), Empresa = p.Empresa, Producto = det.Producto.Truncate(40), EspecificaIdentificador = det.EspecificaIdentificadorId };

                        var key = $"{p.Id}.{p.Cliente}.{Empresa}.{det.Producto}.{det.Identificador}.{det.EspecificaIdentificadorId}";
                        if (det.Identificador == ManejoIdentificadorDb.IdentificadorAuto)
                            DetallesConLoteAsociados[key] = new List<string>();

                        foreach (var dup in det.Duplicados)
                        {
                            dup.Cliente = p.Cliente;
                            dup.Identificador = GetIdentificador(dup.Identificador, dup.Producto);
                            if (dup.Identificador == ManejoIdentificadorDb.IdentificadorProducto)
                                dup.IdEspecificaIdentificador = "S";
                        }
                    }
                }
            }

            var pedidos = _uow.PedidoRepository.GetPedidosWithSaldo(_pedidos);

            foreach (var detalleLpn in _uow.PedidoRepository.GetDetallesPedidosLpn(_pedidos))
            {
                var keyDetalleLpn = $"{detalleLpn.Pedido}.{detalleLpn.Empresa}.{detalleLpn.Cliente}.{detalleLpn.Producto}.{detalleLpn.Identificador}.{detalleLpn.IdEspecificaIdentificador}.{detalleLpn.IdLpnExterno}.{detalleLpn.Tipo}";
                var keyPedido = $"{detalleLpn.Pedido}.{detalleLpn.Empresa}.{detalleLpn.Cliente}";

                DetallesPedidoLpn[keyDetalleLpn] = detalleLpn;

                if (!PedidoDetallesLpn.ContainsKey(keyPedido))
                    PedidoDetallesLpn[keyPedido] = new List<DetallePedidoLpn>();

                PedidoDetallesLpn[keyPedido].Add(detalleLpn);
            }

            foreach (var configuracion in _uow.PedidoRepository.GetAtributosLpn(_pedidos))
            {
                var keyDetallePedidoLpn = $"{configuracion.Pedido}.{configuracion.Cliente}.{configuracion.Empresa}.{configuracion.Producto}.{configuracion.Faixa.ToString("#.###")}.{configuracion.Identificador}.{configuracion.IdEspecificaIdentificador}.{configuracion.IdLpnExterno}.{configuracion.Tipo}";

                if (!DetallePedidoAtributosLpn.ContainsKey(keyDetallePedidoLpn))
                    DetallePedidoAtributosLpn[keyDetallePedidoLpn] = new List<DetallePedidoAtributosLpn>();

                DetallePedidoAtributosLpn[keyDetallePedidoLpn].Add(configuracion);
            }

            foreach (var configuracion in _uow.PedidoRepository.GetAtributos(_pedidos))
            {
                var keyDetallePedido = $"{configuracion.Pedido}.{configuracion.Cliente}.{configuracion.Empresa}.{configuracion.Producto}.{configuracion.Faixa.ToString("#.###")}.{configuracion.Identificador}.{configuracion.IdEspecificaIdentificador}";

                if (!DetallePedidoAtributos.ContainsKey(keyDetallePedido))
                    DetallePedidoAtributos[keyDetallePedido] = new List<DetallePedidoAtributos>();

                DetallePedidoAtributos[keyDetallePedido].Add(configuracion);
            }

            foreach (var p in pedidos)
            {
                var key = $"{p.Id}.{p.Empresa}.{p.Cliente}";
                EditarTipoHabilitado[key] = (p.Total != p.Saldo) ? false : true;
                SaldosTotales[key] = p.Saldo;
                Pedidos[key] = p;
            }

            var detalles = _uow.PedidoRepository.GetDetallesParcialPedidos(keysDetalleParcial.Values);

            foreach (var d in detalles)
            {
                var key = $"{d.Id}.{d.Cliente}.{d.Empresa}.{d.Producto}.{d.Identificador}.{d.EspecificaIdentificadorId}";
                Detalles[key] = d;

                if (d.Identificador != ManejoIdentificadorDb.IdentificadorAuto && d.EspecificaIdentificadorId == "N")
                {
                    var keyAuto = $"{d.Id}.{d.Cliente}.{d.Empresa}.{d.Producto}.{ManejoIdentificadorDb.IdentificadorAuto}.{"N"}";

                    if (!DetallesConLoteAsociados.ContainsKey(keyAuto))
                        DetallesConLoteAsociados[keyAuto] = new List<string>();

                    DetallesConLoteAsociados[keyAuto].Add(key);
                }
            }

            var duplicados = _uow.PedidoRepository.GetDetallesDuplicadosParcial(keysDetalleParcial.Values);

            foreach (var d in duplicados)
            {
                var keyDet = $"{d.Pedido}.{d.Cliente}.{d.Empresa}.{d.Producto}.{d.Identificador}.{d.IdEspecificaIdentificador}";
                var keyDup = $"{d.Pedido}.{d.Cliente}.{d.Empresa}.{d.Producto}.{d.Identificador}.{d.IdEspecificaIdentificador}.{d.IdLineaSistemaExterno}";

                Duplicados[keyDup] = d;

                if (d.Identificador == ManejoIdentificadorDb.IdentificadorAuto || d.IdEspecificaIdentificador == "S")
                {
                    if (!DuplicadosAsociados.ContainsKey(keyDet))
                        DuplicadosAsociados[keyDet] = new List<string>();
                    DuplicadosAsociados[keyDet].Add(keyDup);
                }

                if (d.Identificador != ManejoIdentificadorDb.IdentificadorAuto && d.IdEspecificaIdentificador == "N")
                {
                    var keyDupAuto = $"{d.Pedido}.{d.Cliente}.{d.Empresa}.{d.Producto}.{ManejoIdentificadorDb.IdentificadorAuto}.{"N"}.{d.IdLineaSistemaExterno}";
                    if (!DuplicadosConLoteAsociados.ContainsKey(keyDupAuto))
                        DuplicadosConLoteAsociados[keyDupAuto] = new List<string>();

                    DuplicadosConLoteAsociados[keyDupAuto].Add(keyDup);
                }
            }

            foreach (var atributo in _uow.AtributoRepository.GetAtributosByNombre(keysAtributos.Values))
            {
                Atributos[atributo.Nombre] = atributo;
            }

            foreach (var z in _uow.ZonaRepository.GetZonas())
            {
                Zonas.Add(z.CdZona);
            }
        }

        public virtual List<LpnDetalle> GetDetallesLpn(long lpn)
        {
            if (!_detallesLpn.ContainsKey(lpn))
                return new List<LpnDetalle>();

            return _detallesLpn[lpn];
        }

        public virtual DetallePedido GetDetallePedido(DetallePedido det)
        {
            var key = $"{det.Id}.{det.Cliente}.{det.Empresa}.{det.Producto}.{det.Identificador}.{det.EspecificaIdentificadorId}";
            return Detalles.GetValueOrDefault(key, null);
        }

        public virtual DetallePedido GetDetallePedidoNoEspecifico(DetallePedido det)
        {
            var key = $"{det.Id}.{det.Cliente}.{det.Empresa}.{det.Producto}.{det.Identificador}.N";
            return Detalles.GetValueOrDefault(key, null);
        }

        public virtual DetallePedidoDuplicado GetDuplicado(DetallePedidoDuplicado dup)
        {
            var key = $"{dup.Pedido}.{dup.Cliente}.{dup.Empresa}.{dup.Producto}.{dup.Identificador}.{dup.IdEspecificaIdentificador}.{dup.IdLineaSistemaExterno}";
            return Duplicados.GetValueOrDefault(key, null);
        }

        public virtual DetallePedidoDuplicado GetDuplicado(string key)
        {
            return Duplicados.GetValueOrDefault(key, null);
        }

        public virtual DetallePedidoLpn GetDetalleLpn(DetallePedidoLpn detalleLpn)
        {
            var keyDetalleLpn = $"{detalleLpn.Pedido}.{detalleLpn.Empresa}.{detalleLpn.Cliente}.{detalleLpn.Producto}.{detalleLpn.Identificador}.{detalleLpn.IdEspecificaIdentificador}.{detalleLpn.IdLpnExterno}.{detalleLpn.Tipo}";
            return DetallesPedidoLpn.GetValueOrDefault(keyDetalleLpn, null);
        }

        public virtual List<DetallePedidoLpn> GetDetallesLpn(Pedido pedido)
        {
            var keyPedido = $"{pedido.Id}.{pedido.Empresa}.{pedido.Cliente}";
            return PedidoDetallesLpn.GetValueOrDefault(keyPedido, new List<DetallePedidoLpn>());
        }

        public virtual List<DetallePedidoDuplicado> GetDuplicadosConLoteAsociados(DetallePedidoDuplicado det)
        {
            var duplicados = new List<DetallePedidoDuplicado>();
            var key = $"{det.Pedido}.{det.Cliente}.{det.Empresa}.{det.Producto}.{det.Identificador}.{det.IdEspecificaIdentificador}.{det.IdLineaSistemaExterno}";

            var keyDups = DuplicadosConLoteAsociados.GetValueOrDefault(key, new List<string>());
            foreach (var d in keyDups)
            {
                duplicados.Add(Duplicados.GetValueOrDefault(d, null));
            }
            return duplicados;
        }

        public virtual List<string> GetKeysDuplicadosAsociados(DetallePedido det)
        {
            var key = $"{det.Id}.{det.Cliente}.{det.Empresa}.{det.Producto}.{det.Identificador}.{det.EspecificaIdentificadorId}";
            return DuplicadosAsociados.GetValueOrDefault(key, null); ;
        }

        public virtual List<DetallePedido> GetDetallesConLoteAsociados(DetallePedido det)
        {
            var result = new List<DetallePedido>();
            var key = $"{det.Id}.{det.Cliente}.{det.Empresa}.{det.Producto}.{det.Identificador}.{det.EspecificaIdentificadorId}";
            var keysAsociadas = DetallesConLoteAsociados.GetValueOrDefault(key, null);

            foreach (var k in keysAsociadas)
            {
                result.Add(Detalles.GetValueOrDefault(k, null));
            }

            return result;
        }

        public virtual string GetIdentificador(string identificador, string producto)
        {
            var prod = GetProducto(Empresa, producto);

            if (string.IsNullOrEmpty(identificador))
            {
                if (prod.ManejoIdentificador == ManejoIdentificador.Producto)
                    return ManejoIdentificadorDb.IdentificadorProducto;
                else
                    return ManejoIdentificadorDb.IdentificadorAuto;
            }
            return identificador;
        }

        public virtual Agente GetAgente(string codigo, int empresa, string tipo)
        {
            var cliente = Agentes.GetValueOrDefault($"{tipo}.{codigo}", string.Empty);
            if (string.IsNullOrEmpty(cliente))
                return null;

            return new Agente()
            {
                Codigo = codigo,
                CodigoInterno = cliente,
                Empresa = empresa,
                Tipo = tipo
            };
        }

        public virtual Producto GetProducto(int empresa, string codigo)
        {
            return Productos.GetValueOrDefault(codigo, null);
        }

        public virtual Pedido GetPedido(string pedido, int empresa, string cliente)
        {
            var key = $"{pedido}.{empresa}.{cliente}";
            return Pedidos.GetValueOrDefault(key, null);
        }

        public virtual decimal GetSaldoTotal(string pedido, int empresa, string cliente)
        {
            var key = $"{pedido}.{empresa}.{cliente}";
            return SaldosTotales.GetValueOrDefault(key, null) ?? 0;
        }

        public virtual bool PuedoEditarTipo(string pedido, int empresa, string cliente)
        {
            var key = $"{pedido}.{empresa}.{cliente}";
            return EditarTipoHabilitado.GetValueOrDefault(key, false);
        }

        public virtual bool ExistePedido(string pedido, int empresa, string cliente)
        {
            var key = $"{pedido}.{empresa}.{cliente}";
            return Pedidos.TryGetValue(key, out Pedido ped);
        }

        public virtual bool IsPedidoProduccion(string pedido, int empresa, string cliente)
        {
            var key = $"{pedido}.{empresa}.{cliente}";
            Pedidos.TryGetValue(key, out Pedido ped);

            return string.IsNullOrEmpty(ped.IngresoProduccion);

        }

        public virtual bool ExisteTpExpedicion(string tipo)
        {
            return TiposExpedicion.Contains(tipo);
        }

        public virtual bool ExisteTpPedido(string tipo)
        {
            return TiposPedido.Contains(tipo);
        }

        public virtual bool ExisteRelTpExpdicionPedido(string tipoExpedicion, string tipoPedido)
        {
            return TiposExpedicionPedido.Contains($"{tipoExpedicion}.{tipoPedido}");
        }

        public virtual bool ExistePredio(string predio)
        {
            return Predios.Contains(predio);
        }

        public virtual bool ExisteRuta(short ruta)
        {
            return Rutas.Contains(ruta);
        }

        public virtual bool ExisteCondicionLiberacion(string condicion)
        {
            return CondicionesLiberacion.Contains(condicion);
        }

        public virtual bool ExisteTransportadora(int transportadora)
        {
            return Transportadoras.Contains(transportadora);
        }

        public virtual bool ExisteLpn(string idLpnExterno, string tipo)
        {
            return _lpns.ContainsKey($"{tipo}.{idLpnExterno}");
        }

        public virtual Lpn GetLpn(string idLpnExterno, string tipo)
        {
            return _lpns.GetValueOrDefault($"{tipo}.{idLpnExterno}", null);
        }

        public virtual bool ExisteDetalleLpn(long lpn, string producto, string lote)
        {
            if (!_detallesLpn.ContainsKey(lpn))
                return false;

            return _detallesLpn[lpn]
                .Any(x => x.CodigoProducto == producto && x.Lote == lote);
        }

        public virtual bool ExisteAtributo(string nombre)
        {
            return Atributos.ContainsKey(nombre);
        }

        public virtual bool ExisteAtributoDetalleLpn(string idLpnExterno, string tipoLpn, int empresa, string producto, decimal faixa, string lote, string nombreAtributo)
        {
            var keyDetalleLpnAtributo = $"{idLpnExterno}.{tipoLpn}.{empresa}.{producto}.{faixa.ToString("#.###")}.{lote}.{nombreAtributo}";

            return DetallePedidoLpnAtributos.Contains(keyDetalleLpnAtributo);
        }

        public virtual bool ExisteZona(string zona)
        {
            return Zonas.Contains(zona);
        }

        public virtual LpnTipo GetTipo(string tipo)
        {
            return _tiposLpn.GetValueOrDefault(tipo, null);
        }

        public virtual decimal GetCantidadDetallesLpn(long numeroLPN, string producto, string lote)
        {
            if (!_detallesLpn.ContainsKey(numeroLPN))
                return 0;

            return _detallesLpn[numeroLPN]
                .Where(x => x.CodigoProducto == producto && x.Lote == lote)
                .Sum(x => x.Cantidad - (x.CantidadReserva ?? 0));
        }

        public virtual int GetCantidadDetallesLpn(long numeroLPN)
        {
            if (!_detallesLpn.ContainsKey(numeroLPN))
                return 0;

            int cantidadLpn = _detallesLpn[numeroLPN]
                .GroupBy(x => new { x.CodigoProducto, x.Lote })
                .Count();

            return cantidadLpn;
        }

        public virtual DetallePedidoAtributos GetAtributos(DetallePedidoAtributos configuracion)
        {
            var jsonConfiguracion = GetJson(configuracion);
            var keyDetallePedido = $"{configuracion.Pedido}.{configuracion.Cliente}.{configuracion.Empresa}.{configuracion.Producto}.{configuracion.Faixa.ToString("#.###")}.{configuracion.Identificador}.{configuracion.IdEspecificaIdentificador}";
            var atributos = DetallePedidoAtributos.GetValueOrDefault(keyDetallePedido, new List<DetallePedidoAtributos>());

            foreach (var conf in atributos)
            {
                var jsonConf = GetJson(conf);
                if (jsonConfiguracion == jsonConf)
                    return conf;
            }

            return null;
        }

        public virtual DetallePedidoAtributosLpn GetAtributos(DetallePedidoAtributosLpn configuracion)
        {
            var jsonConfiguracion = GetJson(configuracion);
            var keyDetallePedido = $"{configuracion.Pedido}.{configuracion.Cliente}.{configuracion.Empresa}.{configuracion.Producto}.{configuracion.Faixa.ToString("#.###")}.{configuracion.Identificador}.{configuracion.IdEspecificaIdentificador}.{configuracion.IdLpnExterno}.{configuracion.Tipo}";
            var atributos = DetallePedidoAtributosLpn.GetValueOrDefault(keyDetallePedido, new List<DetallePedidoAtributosLpn>());

            foreach (var conf in atributos)
            {
                var jsonConf = GetJson(conf);
                if (jsonConfiguracion == jsonConf)
                    return conf;
            }

            return null;
        }

        public virtual string GetJson(DetallePedidoAtributos configuracion)
        {
            return JsonConvert.SerializeObject(configuracion.Atributos
                .OrderBy(a => a.IdCabezal)
                .ThenBy(a => a.Nombre)
                .Select(a => new
                {
                    a.IdCabezal,
                    a.Nombre,
                    a.Valor
                }));
        }

        public virtual string GetJson(DetallePedidoAtributosLpn configuracion)
        {
            return JsonConvert.SerializeObject(configuracion.Atributos
                .OrderBy(a => a.IdCabezal)
                .ThenBy(a => a.Nombre)
                .Select(a => new
                {
                    a.IdCabezal,
                    a.Nombre,
                    a.Valor
                }));
        }

        public virtual List<DetallePedidoAtributos> GetAtributosRegistrados(DetallePedido detalle)
        {
            var keyDetallePedido = $"{detalle.Id}.{detalle.Cliente}.{detalle.Empresa}.{detalle.Producto}.{detalle.Faixa.ToString("#.###")}.{detalle.Identificador}.{detalle.EspecificaIdentificadorId}";
            return DetallePedidoAtributos.GetValueOrDefault(keyDetallePedido, new List<DetallePedidoAtributos>());
        }

        public virtual List<DetallePedidoAtributosLpn> GetAtributosLpnRegistrados(DetallePedidoLpn detalle)
        {
            var keyDetalleLpnPedido = $"{detalle.Pedido}.{detalle.Cliente}.{detalle.Empresa}.{detalle.Producto}.{detalle.Faixa.ToString("#.###")}.{detalle.Identificador}.{detalle.IdEspecificaIdentificador}.{detalle.IdLpnExterno}.{detalle.Tipo}";
            return DetallePedidoAtributosLpn.GetValueOrDefault(keyDetalleLpnPedido, new List<DetallePedidoAtributosLpn>());
        }

        public virtual Atributo GetAtributo(string nombre)
        {
            return Atributos.GetValueOrDefault(nombre, null);
        }
    }
}
