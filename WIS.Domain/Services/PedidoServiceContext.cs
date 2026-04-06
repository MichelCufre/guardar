using WIS.Extension;
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
    public class PedidoServiceContext : ServiceContext, IPedidoServiceContext
    {
        protected List<Pedido> _pedidos = new List<Pedido>();

        public HashSet<int> Transportadoras { get; set; } = new HashSet<int>();
        public HashSet<short> Rutas { get; set; } = new HashSet<short>();
        public HashSet<string> Predios { get; set; } = new HashSet<string>();
        public HashSet<string> TiposAgente { get; set; } = new HashSet<string>();
        public HashSet<string> TiposPedido { get; set; } = new HashSet<string>();
        public HashSet<string> TiposExpedicion { get; set; } = new HashSet<string>();
        public HashSet<string> TiposExpedicionPedido { get; set; } = new HashSet<string>();
        public HashSet<string> CondicionesLiberacion { get; set; } = new HashSet<string>();
        public HashSet<string> DetallePedidoLpnAtributos { get; set; } = new HashSet<string>();
        public HashSet<string> Zonas { get; set; } = new HashSet<string>();
        public Dictionary<string, Lpn> Lpns { get; set; } = new Dictionary<string, Lpn>();
        public Dictionary<string, string> Agentes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, Pedido> Pedidos { get; set; } = new Dictionary<string, Pedido>();
        public Dictionary<string, LpnTipo> TiposLpn { get; set; } = new Dictionary<string, LpnTipo>();
        public Dictionary<string, Producto> Productos { get; set; } = new Dictionary<string, Producto>();
        public Dictionary<string, Atributo> Atributos { get; set; } = new Dictionary<string, Atributo>();
        public Dictionary<long, List<LpnDetalle>> DetallesLpn { get; set; } = new Dictionary<long, List<LpnDetalle>>();

        public PedidoServiceContext(IUnitOfWork uow, List<Pedido> pedidos, int userId, int empresa) : base(uow, userId, empresa)
        {
            _pedidos = pedidos;
        }

        public async override Task Load()
        {
            await base.Load();

            var keysAgentes = new Dictionary<string, Agente>();
            var keysProductos = new Dictionary<string, Producto>();
            var keysRutas = new Dictionary<string, Ruta>();

            var keysLpn = new Dictionary<string, Lpn>();
            var keysTipoLpn = new Dictionary<string, LpnTipo>();
            var keysDetallesLpn = new Dictionary<string, LpnDetalle>();
            var keysAtributos = new Dictionary<string, Atributo>();
            var keysDetalleLpnAtributos = new Dictionary<string, LpnDetalleAtributo>();

            var detallesLpn = new Dictionary<string, LpnDetalle>();

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
                Lpns[$"{l.Tipo}.{l.IdExterno}"] = l;
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
                if (!DetallesLpn.ContainsKey(d.NumeroLPN))
                    DetallesLpn[d.NumeroLPN] = new List<LpnDetalle>();

                DetallesLpn[d.NumeroLPN].Add(d);
            }

            foreach (var t in _uow.ManejoLpnRepository.GetTiposLpn(keysTipoLpn.Values))
            {
                TiposLpn[t.Tipo] = t;
            }

            var rutaParam = GetParametro(ParamManager.IE_503_CD_ROTA);
            if (short.TryParse(rutaParam, out short ruta) && !keysRutas.ContainsKey(rutaParam))
                keysRutas.Add(rutaParam, new Ruta() { Id = ruta });

            foreach (var a in _uow.AgenteRepository.GetAgentes(keysAgentes.Values))
            {
                Agentes[($"{a.Tipo}.{a.Codigo}")] = a.CodigoInterno;

                if (a.RutaId.HasValue)
                {
                    var keyRuta = $"{a.RutaId.Value}";
                    keysRutas[keyRuta] = new Ruta() { Id = (short)a.RutaId.Value };
                }
            }

            foreach (var r in _uow.RutaRepository.GetRutas(keysRutas.Values))
            {
                Rutas.Add(r);
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
                {
                    p.Cliente = cliente;
                }
            }

            var pedidos = _uow.PedidoRepository.GetPedidos(_pedidos);
            foreach (var p in pedidos)
            {
                var key = $"{p.Id}.{p.Empresa}.{p.Cliente}";
                Pedidos[key] = p;
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

        public virtual Agente GetAgente(string codigo, int empresa, string tipo)
        {
            var cliente = Agentes.GetValueOrDefault($"{tipo}.{codigo}", string.Empty);

            if (string.IsNullOrEmpty(cliente))
            {
                return null;
            }

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

        public virtual bool ExistePedido(string pedido, int empresa, string cliente)
        {
            var key = $"{pedido}.{empresa}.{cliente}";
            return Pedidos.TryGetValue(key, out Pedido ped);
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

        public virtual bool ExisteTransportista(int transportadora)
        {
            return Transportadoras.Contains(transportadora);
        }

        public virtual bool ExisteLpn(string idLpnExterno, string tipo)
        {
            return Lpns.ContainsKey($"{tipo}.{idLpnExterno}");
        }

        public virtual Lpn GetLpn(string idLpnExterno, string tipo)
        {
            return Lpns.GetValueOrDefault($"{tipo}.{idLpnExterno}", null);
        }

        public virtual bool ExisteDetalleLpn(long nroLpn, string producto, string lote)
        {
            if (!DetallesLpn.ContainsKey(nroLpn))
                return false;

            return DetallesLpn[nroLpn]
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

        public virtual List<LpnDetalle> GetDetallesLpn(long nroLpn)
        {
            if (!DetallesLpn.ContainsKey(nroLpn))
                return new List<LpnDetalle>();

            return DetallesLpn[nroLpn];
        }

        public virtual LpnTipo GetTipoLpn(string tipo)
        {
            return TiposLpn.GetValueOrDefault(tipo, null);
        }

        public virtual decimal GetCantidadDetallesLpn(long nroLpn, string producto, string lote)
        {
            if (!DetallesLpn.ContainsKey(nroLpn))
                return 0;

            return DetallesLpn[nroLpn]
                .Where(x => x.CodigoProducto == producto && x.Lote == lote)
                .Sum(x => x.Cantidad - (x.CantidadReserva ?? 0));
        }

        public virtual int GetCantidadDetallesLpn(long nroLpn)
        {
            if (!DetallesLpn.ContainsKey(nroLpn))
                return 0;

            int cantidadLpn = DetallesLpn[nroLpn]
                .GroupBy(x => new { x.CodigoProducto, x.Lote })
                .Count();

            return cantidadLpn;
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

        public virtual Atributo GetAtributo(string nombre)
        {
            return Atributos.GetValueOrDefault(nombre, null);
        }
    }
}
