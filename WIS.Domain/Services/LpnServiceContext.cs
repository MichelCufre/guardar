using WIS.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services
{
    public class LpnServiceContext : ServiceContext, ILpnServiceContext
    {
        public List<Lpn> _lpns { get; private set; } = new List<Lpn>();

        public List<Lpn> Lpns { get;set;} = new List<Lpn>();
        public List<LpnBarras> LpnBarras { get;set;} = new List<LpnBarras>();
        public HashSet<string> TipoBarrasLpn { get;set;} = new HashSet<string>();
        public Dictionary<string, LpnTipo> TiposLpn { get;set;} = new Dictionary<string, LpnTipo>();
        public Dictionary<string, Atributo> Atributos { get;set;} = new Dictionary<string, Atributo>();
        public Dictionary<string, Producto> Productos { get;set;} = new Dictionary<string, Producto>();
        public Dictionary<short, AtributoValidacion> AtributosValidacion { get;set;} = new Dictionary<short, AtributoValidacion>();
        // Dada la key de un Dominio retorna los detalles del mismo.
        public Dictionary<string, List<DominioDetalle>> Dominios { get;set;} = new Dictionary<string, List<DominioDetalle>>();
        //Atributos definidos para el cabezal del tipo de Lpn
        public Dictionary<string, LpnTipoAtributo> TipoLpnAtributos { get;set;} = new Dictionary<string, LpnTipoAtributo>();
        //Atributos definidos para el detalle del tipo de Lpn
        public Dictionary<string, LpnTipoAtributoDet> TipoLpnAtributosDetalle { get;set;} = new Dictionary<string, LpnTipoAtributoDet>();
        // Dada la key de un tipo de Lpn retorna los atributos definidos asociados al cabezal.
        public Dictionary<string, List<LpnTipoAtributo>> TipoLpnAtributosAsociados { get;set;} = new Dictionary<string, List<LpnTipoAtributo>>();
        // Dada la key de un tipo de Lpn retorna los atributos definidos asociados al detalle.
        public Dictionary<string, List<LpnTipoAtributoDet>> TipoLpnAtributosDetalleAsociados { get;set;} = new Dictionary<string, List<LpnTipoAtributoDet>>();
        // Dada la key de un Atributo retorna las validaciones asociadas al mismo.
        public Dictionary<int, List<AtributoValidacionAsociada>> AtributosValidacionesAsociadas { get;set;} = new Dictionary<int, List<AtributoValidacionAsociada>>();
        // Dada la key de un Lpn retorna las los atributos del cabezal.
        public Dictionary<long, List<LpnAtributo>> LpnAtributosCabezal { get;set;} = new Dictionary<long, List<LpnAtributo>>();

        public LpnServiceContext(IUnitOfWork uow, List<Lpn> lpns, int empresa, int userId) : base(uow, userId, empresa)
        {
            _lpns = lpns;
        }

        public async override Task Load()
        {
            await base.Load();
            var keysTipoLpn = new Dictionary<string, LpnTipo>();
            var keysAtributos = new Dictionary<string, Atributo>();
            var keysProductos = new Dictionary<string, Producto>();
            var keysBarras = new Dictionary<string, LpnBarras>();
            var keysLpns = new Dictionary<string, Lpn>();

            foreach (var lpn in _lpns)
            {
                var keyLpn = $"{lpn.IdExterno}.{lpn.Tipo}";
                keysLpns[keyLpn] = new Lpn() { IdExterno = lpn.IdExterno.Truncate(50), Tipo = lpn.Tipo.Truncate(10) };

                if (string.IsNullOrEmpty(lpn.Tipo))
                    lpn.Tipo = GetParametro(ParamManager.IE_535_TP_LPN_TIPO);

                if (!keysTipoLpn.ContainsKey(lpn.Tipo))
                    keysTipoLpn[lpn.Tipo] = new LpnTipo() { Tipo = lpn.Tipo.Truncate(10) };

                foreach (var b in lpn.BarrasSinDefinir)
                {
                    if (string.IsNullOrEmpty(b.Tipo))
                        b.Tipo = BarcodeDb.TIPO_LPN_CB;

                    if (!keysBarras.ContainsKey(b.CodigoBarras))
                        keysBarras[b.CodigoBarras] = new LpnBarras() { CodigoBarras = b.CodigoBarras.Truncate(100) };
                }

                foreach (var d in lpn.Detalles)
                {
                    var keyProducto = $"{d.CodigoProducto}.{Empresa}";
                    if (!keysProductos.ContainsKey(keyProducto))
                        keysProductos[keyProducto] = new Producto() { Codigo = d.CodigoProducto.Truncate(40), CodigoEmpresa = Empresa };
                }
            }

            LpnBarras = _uow.ManejoLpnRepository.GetLpnBarrasEmpresa(Empresa, keysBarras.Values).ToList();
            TipoBarrasLpn = _uow.ManejoLpnRepository.GetLpnTipoBarras().ToHashSet();

            Lpns = _uow.ManejoLpnRepository.GetLpns(keysLpns.Values).ToList();
            TiposLpn = _uow.ManejoLpnRepository.GetTiposLpn(keysTipoLpn.Values).ToDictionary(tp => tp.Tipo, tp => tp);

            var atributosCabezales = _uow.ManejoLpnRepository.GetLpnAtributosCabezal(Lpns);

            foreach (var at in atributosCabezales)
            {
                if (!LpnAtributosCabezal.ContainsKey(at.NumeroLpn))
                    LpnAtributosCabezal[at.NumeroLpn] = new List<LpnAtributo>();

                LpnAtributosCabezal[at.NumeroLpn].Add(at);
            }

            var tpLpnAtributos = _uow.ManejoLpnRepository.GetTipoLpnAtributos(TiposLpn.Values);

            foreach (var at in tpLpnAtributos)
            {
                var key = $"{at.TipoLpn}.{at.NombreAtributo}";
                TipoLpnAtributos[key] = at;

                if (!TipoLpnAtributosAsociados.ContainsKey(at.TipoLpn))
                    TipoLpnAtributosAsociados[at.TipoLpn] = new List<LpnTipoAtributo>();

                TipoLpnAtributosAsociados[at.TipoLpn].Add(at);

                if (!keysAtributos.ContainsKey(at.NombreAtributo))
                    keysAtributos[at.NombreAtributo] = new Atributo()
                    {
                        Id = at.IdAtributo,
                        Nombre = at.NombreAtributo,
                    };
            }

            var tpLpnAtributosDetalle = _uow.ManejoLpnRepository.GetTipoLpnAtributosDetalle(TiposLpn.Values);
            foreach (var atd in tpLpnAtributosDetalle)
            {
                var key = $"{atd.TipoLpn}.{atd.NombreAtributo}";
                TipoLpnAtributosDetalle[key] = atd;

                if (!TipoLpnAtributosDetalleAsociados.ContainsKey(atd.TipoLpn))
                    TipoLpnAtributosDetalleAsociados[atd.TipoLpn] = new List<LpnTipoAtributoDet>();

                TipoLpnAtributosDetalleAsociados[atd.TipoLpn].Add(atd);

                if (!keysAtributos.ContainsKey(atd.NombreAtributo))
                    keysAtributos[atd.NombreAtributo] = new Atributo()
                    {
                        Id = atd.IdAtributo,
                        Nombre = atd.NombreAtributo,
                    };
            }

            Atributos = _uow.AtributoRepository.GetAtributos(keysAtributos.Values).ToDictionary(a => a.Nombre, a => a);
            AtributosValidacion = _uow.AtributoRepository.GetValidaciones().ToDictionary(a => a.Id, a => a);

            var validacionesAsociadas = _uow.AtributoRepository.GetValidacionesAsociadas(Atributos.Values);
            foreach (var v in validacionesAsociadas)
            {
                if (!AtributosValidacionesAsociadas.ContainsKey(v.IdAtributo))
                    AtributosValidacionesAsociadas[v.IdAtributo] = new List<AtributoValidacionAsociada>();

                AtributosValidacionesAsociadas[v.IdAtributo].Add(v);
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProductos.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);
                Productos[p.Codigo] = p;
            }

            var atbDominios = Atributos.Values.Where(a => a.IdTipo == TipoAtributoDb.DOMINIO)
                .Select(a => new DominioDetalle() { Codigo = a.CodigoDominio });

            var dominios = _uow.DominioRepository.GetDetallesDominio(atbDominios);
            foreach (var d in dominios)
            {
                if (!Dominios.ContainsKey(d.Codigo))
                    Dominios[d.Codigo] = new List<DominioDetalle>();
                Dominios[d.Codigo].Add(d);
            }
        }

        public virtual bool ExisteLpnActivo(string idExterno, string tipo)
        {
            return Lpns
                .Any(l => l.IdExterno == idExterno
                    && l.Tipo == tipo
                    && l.Estado != EstadosLPN.Finalizado);
        }

        public virtual bool ExisteLpnBarraActivo(string cdBarras)
        {
            return LpnBarras
                .Any(c => c.CodigoBarras == cdBarras
                    && c.EstadoLpn != EstadosLPN.Finalizado);
        }

        public virtual bool ExisteTipoBarra(string tipo)
        {
            return TipoBarrasLpn.Contains(tipo);
        }

        public virtual LpnTipo GetTipoLpn(string tipo)
        {
            return TiposLpn.GetValueOrDefault(tipo, null);
        }

        public virtual Producto GetProducto(int empresa, string codigo)
        {
            return Productos.GetValueOrDefault(codigo, null);
        }

        public virtual Atributo GetAtributo(string nombre)
        {
            return Atributos.GetValueOrDefault(nombre, null);
        }

        public virtual AtributoValidacion GetAtributoValidacion(short idValidacion)
        {
            return AtributosValidacion.GetValueOrDefault(idValidacion, null);
        }

        public virtual List<AtributoValidacionAsociada> GetValidacionesAsociadas(int idAtributo)
        {
            return AtributosValidacionesAsociadas.GetValueOrDefault(idAtributo, new List<AtributoValidacionAsociada>());
        }

        public virtual bool ExisteTipoLpnAtributo(string tipo, string nombreAtributo)
        {
            var key = $"{tipo}.{nombreAtributo}";
            return TipoLpnAtributos.TryGetValue(key, out LpnTipoAtributo r);
        }

        public virtual bool ExisteTipoLpnAtributoDet(string tipo, string nombreAtributo)
        {
            var key = $"{tipo}.{nombreAtributo}";
            return TipoLpnAtributosDetalle.TryGetValue(key, out LpnTipoAtributoDet r);
        }

        public virtual bool AnyTipoLpnAtributoDet(string tipo)
        {
            return TipoLpnAtributosDetalle.Any();
        }

        public virtual bool ExistenAtributosFaltantes(string tipo, List<string> atributos)
        {
            var atributosAsociados = TipoLpnAtributosAsociados.GetValueOrDefault(tipo, new List<LpnTipoAtributo>());
            var faltantesRqueridos = atributosAsociados
                .Where(a => !atributos.Contains(a.NombreAtributo)
                    && a.ValidoInterfaz == "S"
                    && (a.Requerido == "S" && a.EstadoInicial == EstadoLpnAtributo.Pendiente))
                .ToList();
            return (faltantesRqueridos != null && faltantesRqueridos.Count > 0);
        }

        public virtual bool ExistenAtributosDetFaltantes(string tipo, List<string> atributos)
        {
            var atributosAsociados = TipoLpnAtributosDetalleAsociados.GetValueOrDefault(tipo, new List<LpnTipoAtributoDet>());
            var faltantesRequeridos = atributosAsociados
                .Where(a => !atributos.Contains(a.NombreAtributo)
                    && a.ValidoInterfaz == "S"
                    && (a.Requerido == "S" && a.EstadoInicial == EstadoLpnAtributo.Pendiente))
                .ToList();

            return (faltantesRequeridos != null && faltantesRequeridos.Count > 0);
        }

        public virtual bool ExisteDominio(string cdDominio, string nuDominio)
        {
            var dominios = Dominios.GetValueOrDefault(cdDominio, new List<DominioDetalle>());
            var dominio = dominios.FirstOrDefault(d => d.Id == nuDominio);

            return dominio != null;
        }

        public virtual Lpn GetUltimoLpn(string idExterno, string tipo)
        {
            return Lpns.Where(l => l.IdExterno == idExterno
                    && l.Tipo == tipo)
                .OrderByDescending(l => l.NumeroLPN)
                .FirstOrDefault();
        }

        public virtual List<LpnAtributo> GetAtributosCabezal(long numeroLPN)
        {
            return LpnAtributosCabezal.GetValueOrDefault(numeroLPN, new List<LpnAtributo>());
        }
    }
}
