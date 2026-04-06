using WIS.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services
{
    public class ReferenciaRecepcionServiceContext : ServiceContext, IReferenciaRecepcionServiceContext
    {
        protected List<ReferenciaRecepcion> _referencias = new List<ReferenciaRecepcion>();

        public HashSet<string> TiposAgente { get; set; } = new HashSet<string>();
        public HashSet<Stock> SeriesExistentes { get; set; } = new HashSet<Stock>();
        public HashSet<string> TiposReferencia { get; set; } = new HashSet<string>();
        public HashSet<string> TiposReferenciaRecepcion { get; set; } = new HashSet<string>();
        public HashSet<string> TiposReferenciaAgenteRecepcion { get; set; } = new HashSet<string>();
        public HashSet<string> Predios { get; set; } = new HashSet<string>();
        public HashSet<string> Monedas { get; set; } = new HashSet<string>();
        public Dictionary<string, string> Agentes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, Producto> Productos { get; set; } = new Dictionary<string, Producto>();
        public Dictionary<string, ReferenciaRecepcion> Referencias { get; set; } = new Dictionary<string, ReferenciaRecepcion>();

        public ReferenciaRecepcionServiceContext(IUnitOfWork uow, List<ReferenciaRecepcion> referencias, int userId, int empresa) : base(uow, userId, empresa)
        {
            _referencias = referencias;
        }


        public async override Task Load()
        {
            await base.Load();

            var keysAgentes = new Dictionary<string, Agente>();
            var keysProductos = new Dictionary<string, Producto>();
            var keysProductoSerie = new Dictionary<string, Producto>();
            var keysReferencias = new Dictionary<string, ReferenciaRecepcion>();

            foreach (var ta in _uow.AgenteRepository.GetTiposAgentes())
            {
                TiposAgente.Add(ta);
            }

            foreach (var tr in _uow.ReferenciaRecepcionRepository.GetReferenciaRecepcionTipos())
            {
                TiposReferencia.Add(tr.Tipo);
            }

            foreach (var ert in _uow.RecepcionTipoRepository.GetRecepcionTiposHabilitadosByEmpresa(Empresa))
            {
                var tipoRecepcion = ert.RecepcionTipoInterno;
                var tipoReferencia = tipoRecepcion.TipoReferencia;
                TiposReferenciaRecepcion.Add(tipoReferencia);
                TiposReferenciaAgenteRecepcion.Add($"{tipoReferencia}.{tipoRecepcion.TipoAgente}");
            }

            foreach (var p in _uow.PredioRepository.GetPrediosUsuario(UserId))
            {
                Predios.Add(p.Numero);
            }

            foreach (var m in _uow.MonedaRepository.GetMonedas())
            {
                Monedas.Add(m.Codigo);
            }

            foreach (var r in _referencias)
            {
                var keyAgente = $"{r.TipoAgente}.{r.CodigoAgente}.{Empresa}";
                keysAgentes[keyAgente] = new Agente() { Tipo = r.TipoAgente.Truncate(3), Codigo = r.CodigoAgente.Truncate(40), Empresa = Empresa };

                foreach (var d in r.Detalles)
                {
                    var keyProducto = $"{d.CodigoProducto}.{Empresa}";
                    keysProductos[keyProducto] = new Producto() { Codigo = d.CodigoProducto.Truncate(40), CodigoEmpresa = Empresa };
                }
            }

            foreach (var a in _uow.AgenteRepository.GetAgentes(keysAgentes.Values))
            {
                Agentes[($"{a.Tipo}.{a.Codigo}")] = a.CodigoInterno;
            }

            foreach (var r in _referencias)
            {
                var cliente = Agentes.GetValueOrDefault($"{r.TipoAgente}.{r.CodigoAgente}", string.Empty);

                var keyReferencia = $"{r.Numero}.{r.IdEmpresa}.{r.TipoReferencia}.{cliente}";
                keysReferencias[keyReferencia] = new ReferenciaRecepcion()
                {
                    Numero = r.Numero.Truncate(20),
                    IdEmpresa = r.IdEmpresa,
                    TipoReferencia = r.TipoReferencia.Truncate(6),
                    CodigoCliente = cliente
                };
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProductos.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);
                Productos[p.Codigo] = p;

                if (p.ManejoIdentificador == ManejoIdentificador.Serie)
                    keysProductoSerie[p.Codigo] = p;
            }

            foreach (var r in _referencias)
            {
                var cliente = Agentes.GetValueOrDefault($"{r.TipoAgente}.{r.CodigoAgente}", string.Empty);

                if (!string.IsNullOrEmpty(cliente))
                {
                    r.CodigoCliente = cliente;
                }
            }

            var referencias = _uow.ReferenciaRecepcionRepository.GetReferencias(keysReferencias.Values);

            foreach (var r in referencias)
            {
                var key = $"{r.Numero}.{r.IdEmpresa}.{r.TipoReferencia}.{r.CodigoCliente}";
                Referencias[key] = r;
            }

            if (keysProductoSerie.Count > 0)
                SeriesExistentes = _uow.StockRepository.GetLotesExistente(keysProductoSerie.Values).ToHashSet();
        }

        public virtual bool ExisteTipoAgente(string tipoAgente)
        {
            return TiposAgente.Contains(tipoAgente);
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

        public virtual ReferenciaRecepcion GetReferencia(string referencia, int empresa, string tipoReferencia, string cliente)
        {
            var key = $"{referencia}.{empresa}.{tipoReferencia}.{cliente}";
            return Referencias.GetValueOrDefault(key, null);
        }

        public virtual bool ExisteReferencia(string referencia, int empresa, string tipoReferencia, string cliente)
        {
            return GetReferencia(referencia, empresa, tipoReferencia, cliente) != null;
        }

        public virtual bool ExisteTipoReferencia(string tipo)
        {
            return TiposReferencia.Contains(tipo);
        }

        public virtual bool ExisteTpRefTpRecepcion(string tipoReferencia)
        {
            return TiposReferenciaRecepcion.Contains(tipoReferencia);
        }

        public virtual bool ExisteTpRefTpAgente(string tipoReferencia, string tipoAgente)
        {
            return TiposReferenciaAgenteRecepcion.Contains($"{tipoReferencia}.{tipoAgente}");
        }

        public virtual bool ExistePredio(string predio)
        {
            return Predios.Contains(predio);
        }

        public virtual bool ExisteMoneda(string moneda)
        {
            return Monedas.Contains(moneda);
        }

        public virtual bool ExisteSerie(string codigoProducto, string identificador)
        {
            return SeriesExistentes.Any(s => s.Producto == codigoProducto && s.Identificador == identificador);
        }
    }
}