using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.Services.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class AlmacenamientoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly AlmacenamientoMapper _mapper;
        protected readonly EtiquetaLoteMapper _mapperEtiqueta;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public AlmacenamientoRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new AlmacenamientoMapper();
            this._mapperEtiqueta = new EtiquetaLoteMapper();
            this._dapper = dapper;
        }

        public virtual SugerenciaAlmacenamiento SugerirUbicacion(CriterioAlmacenaje criterio, DateTime inicioCalculo, bool sugerirDesdeUltimaSugerencia, bool guardarDetalle, long nuTransaccionDB, int nuEtiqueta, bool isOtraSugerencia)
        {
            var sugerencia = null as T_ALM_SUGERENCIA;
            var asociacion = GetEstrategiaAlmacenaje(criterio);

            if (asociacion != null)
            {
                var ultimaSugerencia = GetUltimaSugerenciaNoAprobada(criterio, asociacion);

                if ((ultimaSugerencia?.Estado ?? "") == AlmacenamientoDb.ESTADO_PENDIENTE)
                    return ultimaSugerencia;

                if (!isOtraSugerencia)
                {
                    ultimaSugerencia = null;
                }

                var orden = sugerirDesdeUltimaSugerencia ? (ultimaSugerencia?.Instancia?.Orden ?? 0) : (short)0;
                var logicas = GetLogicasPendientes(asociacion, orden);

                for (int i = 0; i < logicas.Count(); i++)
                {
                    var logica = logicas.ElementAt(i);

                    ultimaSugerencia = sugerirDesdeUltimaSugerencia ? ((ultimaSugerencia?.Instancia?.Id ?? -1) != logica.Id ? null : ultimaSugerencia) : null;
                    sugerencia = SugerirUbicacion(criterio, inicioCalculo, asociacion, logica, ultimaSugerencia, nuTransaccionDB, nuEtiqueta);

                    if (sugerencia != null)
                    {
                        UpdateStockTransitoEntrada(criterio.Productos, sugerencia.CD_ENDERECO_SUGERIDO, false, nuTransaccionDB);

                        if (guardarDetalle)
                            sugerencia = AddSugerencia(sugerencia, criterio.Productos, true);
                        else
                            sugerencia = AddSugerencia(sugerencia);

                        break;
                    }
                }
            }
            else
            {
                return GetUltimaSugerenciaPendiente(criterio);
            }

            return _mapper.Map(sugerencia);
        }

        public virtual void RechazarSugerenciaParaEtiqueta(SugerenciaAlmacenamiento sugerencia, int nuEtiqueta, string cdMotivoRechazo, long nuTransaccionDB)
        {
            var productos = GetProductosAlmacenajeFromEtiqueta(nuEtiqueta);

            DesvincularSugerenciaParaEtiqueta(nuEtiqueta, nuTransaccionDB);
            RechazarSugerencia(sugerencia, cdMotivoRechazo, productos, nuTransaccionDB);
        }

        public virtual void CancelarSugerenciaParaProducto(SugerenciaAlmacenamiento sugerencia, int empresa, string producto, decimal faixa, string lote, decimal cantidad, long nuTransaccionDB)
        {
            var productos = new List<ProductoAlmacenaje>
            {
                new ProductoAlmacenaje
                {
                    Empresa = empresa,
                    Codigo = producto,
                    Faixa = faixa,
                    Lote = lote,
                    CantidadSeparar = cantidad,
                }
            };

            CancelarSugerencia(sugerencia, productos, nuTransaccionDB);
        }

        public virtual void RechazarSugerencia(SugerenciaAlmacenamiento sugerencia, string cdMotivoRechazo, List<ProductoAlmacenaje> productos, long nuTransaccionDB)
        {
            var entity = GetSugerenciaAlmacenamientoSinDetalles(sugerencia);

            entity.CD_MOTVO_RECHAZO = cdMotivoRechazo;
            entity.CD_ESTADO = AlmacenamientoDb.ESTADO_RECHAZADO;
            entity.DT_UPDROW = DateTime.Now;
            entity.CD_FUNCIONARIO = _userId;
            entity.NU_TRANSACCION = nuTransaccionDB;

            UpdateStockTransitoEntrada(productos, sugerencia.UbicacionSugerida, true, nuTransaccionDB);

            _context.T_ALM_SUGERENCIA.Attach(entity);
            _context.Entry<T_ALM_SUGERENCIA>(entity).State = EntityState.Modified;
        }

        public virtual void CancelarSugerencia(SugerenciaAlmacenamiento sugerencia, List<ProductoAlmacenaje> productos, long nuTransaccionDB)
        {
            var entity = GetSugerenciaAlmacenamiento(sugerencia);

            UpdateStockTransitoEntrada(productos, sugerencia.UbicacionSugerida, true, nuTransaccionDB);

            if ((entity.T_ALM_SUGERENCIA_DET?.Count ?? 0) > 0)
            {
                foreach (var det in entity.T_ALM_SUGERENCIA_DET)
                {
                    _context.T_ALM_SUGERENCIA_DET.Remove(det);
                }
            }

            _context.SaveChanges();
            _context.T_ALM_SUGERENCIA.Remove(entity);
        }

        public virtual void AprobarSugerenciaParaEtiqueta(SugerenciaAlmacenamiento sugerencia, int nuEtiqueta, long nuTransaccionDB)
        {
            var productos = GetProductosAlmacenajeFromEtiqueta(nuEtiqueta);

            AprobarSugerencia(sugerencia, productos, false, nuTransaccionDB);
        }

        public virtual void AprobarSugerenciaParaProducto(SugerenciaAlmacenamiento sugerencia, int empresa, string producto, decimal faixa, string lote, decimal cantidad, long nuTransaccionDB, DateTime? vencimiento)
        {
            var productos = new List<ProductoAlmacenaje>
            {
                new ProductoAlmacenaje
                {
                    Empresa = empresa,
                    Codigo = producto,
                    Faixa = faixa,
                    Lote = lote,
                    CantidadSeparar = cantidad,
                    Vencimiento = vencimiento,
                    UbicacionDestino = sugerencia.UbicacionSugerida,
                }
            };

            AprobarSugerencia(sugerencia, productos, false, nuTransaccionDB);
        }

        public virtual void AprobarSugerencia(SugerenciaAlmacenamiento sugerencia, List<ProductoAlmacenaje> productos, bool actualizaTransitoEntrada, long nuTransaccionDB)
        {
            var entity = GetSugerenciaAlmacenamientoSinDetalles(sugerencia);

            entity.CD_ESTADO = AlmacenamientoDb.ESTADO_APROBADO;
            entity.DT_UPDROW = DateTime.Now;
            entity.CD_FUNCIONARIO = _userId;
            entity.NU_TRANSACCION = nuTransaccionDB;

            if (productos != null && productos.Count() > 0)
            {
                var detallesSugerencia = GetDetallesSugerenciaAlmacenamiento(sugerencia, productos);
                if (detallesSugerencia != null)
                {
                    foreach (var detalleSugerencia in detallesSugerencia)
                    {
                        detalleSugerencia.CD_ESTADO = AlmacenamientoDb.ESTADO_APROBADO;
                        detalleSugerencia.DT_UPDROW = DateTime.Now;
                        detalleSugerencia.CD_FUNCIONARIO = _userId;
                        detalleSugerencia.NU_TRANSACCION = nuTransaccionDB;
                        _context.T_ALM_SUGERENCIA_DET.Attach(detalleSugerencia);
                        _context.Entry<T_ALM_SUGERENCIA_DET>(detalleSugerencia).State = EntityState.Modified;
                    }
                }
            }

            if (actualizaTransitoEntrada)
            {
                UpdateStockTransitoEntrada(productos, sugerencia.UbicacionSugerida, true, nuTransaccionDB);
            }

            _context.T_ALM_SUGERENCIA.Attach(entity);
            _context.Entry<T_ALM_SUGERENCIA>(entity).State = EntityState.Modified;
        }

        protected virtual AsociacionEstrategia GetEstrategiaAlmacenaje(CriterioAlmacenaje criterio)
        {
            var asociaciones = _context.T_ALM_ASOCIACION
                            .AsNoTracking()
                            .Where(a => a.NU_PREDIO == criterio.Predio);

            if (!string.IsNullOrEmpty(criterio.Operativa?.Codigo)
               && !string.IsNullOrEmpty(criterio.Operativa?.Tipo))
            {
                asociaciones = asociaciones
                    .Where(a => a.CD_ALM_OPERATIVA_ASOCIABLE == criterio.Operativa.Codigo
                        && a.TP_ALM_OPERATIVA_ASOCIABLE == criterio.Operativa.Tipo);
            }

            var asociacionesProducto = asociaciones.Join(_context.T_ALM_DET_PRODUTO_TEMP.AsNoTracking(),
                a => new { a.CD_PRODUTO, a.CD_EMPRESA },
                almDet => new { almDet.CD_PRODUTO, CD_EMPRESA = (int?)almDet.CD_EMPRESA },
                (a, almDet) => a
                );

            if (asociacionesProducto?.Count() > 0 && criterio.Productos.GroupBy(x => x.Codigo).Count() == 1)
            {
                asociaciones = asociacionesProducto;
            }
            else
            {
                var asociacionesGrupo = asociaciones
                    .Where(a => !string.IsNullOrEmpty(a.CD_GRUPO)
                        && a.CD_GRUPO == criterio.Grupo);

                if (asociacionesGrupo.Count() > 0)
                {
                    asociaciones = asociacionesGrupo;
                }
                else
                {
                    asociaciones = asociaciones
                        .Where(a => !string.IsNullOrEmpty(a.CD_CLASSE)
                            && a.CD_CLASSE == criterio.Clase);
                }
            }

            if (!string.IsNullOrEmpty(criterio.Operativa?.Codigo)
                && !string.IsNullOrEmpty(criterio.Operativa?.Tipo))
            {
                asociaciones = asociaciones
                    .Where(a => a.CD_ALM_OPERATIVA_ASOCIABLE == criterio.Operativa.Codigo
                        && a.TP_ALM_OPERATIVA_ASOCIABLE == criterio.Operativa.Tipo);
            }

            if (asociaciones.Count() > 1)
                return null;

            return _mapper.Map(asociaciones.FirstOrDefault());
        }

        protected virtual List<InstanciaLogica> GetLogicasPendientes(AsociacionEstrategia asociacion, short orden)
        {
            var resultado = new List<InstanciaLogica>();

            var logicas = _context.T_ALM_LOGICA_INSTANCIA
                .Include("T_ALM_LOGICA_INSTANCIA_PARAM")
                .Include("T_ALM_LOGICA_INSTANCIA_PARAM.T_ALM_PARAMETRO")
                .AsNoTracking()
                .Where(li => li.NU_ALM_ESTRATEGIA == asociacion.Estrategia
                    && li.NU_ORDEN >= orden)
                .OrderBy(li => li.NU_ORDEN);

            foreach (var logica in logicas)
            {
                resultado.Add(_mapper.Map(logica));
            }

            return resultado;
        }

        public virtual List<ProductoAlmacenaje> GetProductosAlmacenajeFromEtiqueta(int nuEtiqueta)
        {

            return _context.T_DET_ETIQUETA_LOTE
                .Include("T_ETIQUETA_LOTE")
                .Include("T_PRODUTO")
                .GroupJoin(_context.T_PRODUTO_PALLET,
                    de => new { de.CD_EMPRESA, de.CD_PRODUTO, de.CD_FAIXA, CD_PALLET = de.T_ETIQUETA_LOTE.CD_PALLET ?? -1 },
                    pp => new { pp.CD_EMPRESA, pp.CD_PRODUTO, pp.CD_FAIXA, pp.CD_PALLET },
                    (de, pp) => new { DetalleEtiqueta = de, ProductoPallet = pp })
                .SelectMany(
                    depp => depp.ProductoPallet.DefaultIfEmpty(),
                    (depp, pp) => new { DetalleEtiqueta = depp.DetalleEtiqueta, ProductoPallet = pp })
                .AsNoTracking()
                .Where(d => d.DetalleEtiqueta.NU_ETIQUETA_LOTE == nuEtiqueta)
                .Select(d => new ProductoAlmacenaje
                {
                    Alto = d.DetalleEtiqueta.T_PRODUTO.VL_ALTURA,
                    Ancho = d.DetalleEtiqueta.T_PRODUTO.VL_LARGURA,
                    CantidadSeparar = d.DetalleEtiqueta.QT_PRODUTO ?? 0,
                    Codigo = d.DetalleEtiqueta.CD_PRODUTO,
                    Empresa = d.DetalleEtiqueta.CD_EMPRESA,
                    Faixa = d.DetalleEtiqueta.CD_FAIXA,
                    Lote = d.DetalleEtiqueta.NU_IDENTIFICADOR,
                    Profundidad = d.DetalleEtiqueta.T_PRODUTO.VL_PROFUNDIDADE,
                    Vencimiento = d.DetalleEtiqueta.DT_FABRICACAO,
                    Volumen = d.DetalleEtiqueta.T_PRODUTO.VL_CUBAGEM,
                    Peso = d.DetalleEtiqueta.T_PRODUTO.PS_BRUTO,
                    UnidadesPorPallet = d.ProductoPallet.QT_UNIDADES,
                    Clase = d.DetalleEtiqueta.T_PRODUTO.CD_CLASSE,
                })
                .ToList();
        }

        public virtual List<ProductoAlmacenaje> GetProductosAlmacenajeFromEtiqueta(int nuEtiqueta, int userId, string producto = "", string lote = "", DateTime? vencimiento = null, decimal CantidadAlmacenar = 0)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var detalles = _context.T_DET_ETIQUETA_LOTE
                .Include("T_ETIQUETA_LOTE")
                .Include("T_PRODUTO")
                .GroupJoin(_context.T_PRODUTO_PALLET,
                    de => new { de.CD_EMPRESA, de.CD_PRODUTO, de.CD_FAIXA, CD_PALLET = de.T_ETIQUETA_LOTE.CD_PALLET ?? -1 },
                    pp => new { pp.CD_EMPRESA, pp.CD_PRODUTO, pp.CD_FAIXA, pp.CD_PALLET },
                    (de, pp) => new { DetalleEtiqueta = de, ProductoPallet = pp })
                .SelectMany(
                    depp => depp.ProductoPallet.DefaultIfEmpty(),
                    (depp, pp) => new { DetalleEtiqueta = depp.DetalleEtiqueta, ProductoPallet = pp })
                .AsNoTracking()
                .Where(d => d.DetalleEtiqueta.NU_ETIQUETA_LOTE == nuEtiqueta
                && (string.IsNullOrEmpty(producto)
                    || ((!string.IsNullOrEmpty(producto) && d.DetalleEtiqueta.CD_PRODUTO == producto) && d.DetalleEtiqueta.NU_IDENTIFICADOR == lote)))
                .Select(d => new ProductoAlmacenaje
                {
                    Etiqueta = nuEtiqueta,
                    UserId = userId,
                    Alto = d.DetalleEtiqueta.T_PRODUTO.VL_ALTURA,
                    Ancho = d.DetalleEtiqueta.T_PRODUTO.VL_LARGURA,
                    CantidadSeparar = !string.IsNullOrEmpty(producto) ? CantidadAlmacenar : d.DetalleEtiqueta.QT_PRODUTO ?? 0,
                    Codigo = d.DetalleEtiqueta.CD_PRODUTO,
                    Empresa = d.DetalleEtiqueta.CD_EMPRESA,
                    Faixa = d.DetalleEtiqueta.CD_FAIXA,
                    Lote = d.DetalleEtiqueta.NU_IDENTIFICADOR,
                    Profundidad = d.DetalleEtiqueta.T_PRODUTO.VL_PROFUNDIDADE,
                    Vencimiento = vencimiento != null ? vencimiento : d.DetalleEtiqueta.DT_FABRICACAO,
                    Volumen = d.DetalleEtiqueta.T_PRODUTO.VL_CUBAGEM,
                    Peso = d.DetalleEtiqueta.T_PRODUTO.PS_BRUTO,
                    UnidadesPorPallet = d.ProductoPallet.QT_UNIDADES,
                    Clase = d.DetalleEtiqueta.T_PRODUTO.CD_CLASSE,
                    UnidadBulto = d.DetalleEtiqueta.T_PRODUTO.QT_UND_BULTO,
                })
                .ToList();

            _dapper.BulkInsert(connection, tran, detalles, "T_ALM_DET_PRODUTO_TEMP", new Dictionary<string, Func<ProductoAlmacenaje, ColumnInfo>>
            {
                { "NU_ETIQUETA_LOTE" , x => new ColumnInfo(x.Etiqueta)},
                { "USERID" , x => new ColumnInfo(x.UserId)},
                { "VL_ALTURA" , x => new ColumnInfo(x.Alto,DbType.Decimal)},
                { "VL_LARGURA" , x => new ColumnInfo(x.Ancho,DbType.Decimal)},
                { "QT_ESTOQUE" , x => new ColumnInfo(x.CantidadSeparar,DbType.Decimal)},
                { "CD_PRODUTO" , x => new ColumnInfo(x.Codigo)},
                { "CD_EMPRESA" , x => new ColumnInfo(x.Empresa)},
                { "CD_FAIXA" , x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR" , x => new ColumnInfo(x.Lote)},
                { "VL_PROFUNDIDADE" , x => new ColumnInfo(x.Profundidad,DbType.Decimal)},
                { "DT_FABRICACAO" , x => new ColumnInfo(x.Vencimiento,DbType.DateTime)},
                { "VL_CUBAGEM" , x => new ColumnInfo(x.Volumen,DbType.Decimal)},
                { "PS_BRUTO" , x => new ColumnInfo(x.Peso,DbType.Decimal)},
                { "QT_UNIDADES" , x => new ColumnInfo(x.UnidadesPorPallet,DbType.Decimal)},
                { "QT_UND_BULTO" , x => new ColumnInfo(x.UnidadBulto)},
                { "CD_CLASSE" , x => new ColumnInfo(x.Clase)},
            });
            _context.SaveChanges();
            return detalles;
        }

        public virtual ProductoAlmacenaje GetProductoAlmacenajeFromProducto(int empresa, string producto, decimal faixa, string lote, decimal cantidadSeparar, decimal cantidadClasificada, decimal cantidadOriginal, short? pallet, DateTime? vencimiento)
        {
            return _context.T_PRODUTO
                .Where(p => p.CD_EMPRESA == empresa
                    && p.CD_PRODUTO == producto)
                .GroupJoin(_context.T_PRODUTO_PALLET.Where(pp => pp.CD_PALLET == pallet && pp.CD_FAIXA == faixa),
                    p => new { p.CD_EMPRESA, p.CD_PRODUTO },
                    pp => new { pp.CD_EMPRESA, pp.CD_PRODUTO },
                    (p, pp) => new { Producto = p, ProductoPallet = pp })
                .SelectMany(
                    ppp => ppp.ProductoPallet.DefaultIfEmpty(),
                    (ppp, pp) => new { Producto = ppp.Producto, ProductoPallet = pp })
                .AsNoTracking()
                .Select(ppp => new ProductoAlmacenaje
                {
                    Alto = ppp.Producto.VL_ALTURA,
                    Ancho = ppp.Producto.VL_LARGURA,
                    CantidadSeparar = cantidadSeparar,
                    CantidadClasificada = cantidadClasificada,
                    CantidadAuditada = cantidadOriginal,
                    Codigo = ppp.Producto.CD_PRODUTO,
                    Empresa = ppp.Producto.CD_EMPRESA,
                    Faixa = faixa,
                    Lote = lote,
                    Profundidad = ppp.Producto.VL_PROFUNDIDADE,
                    Vencimiento = vencimiento,
                    Volumen = ppp.Producto.VL_CUBAGEM,
                    Peso = ppp.Producto.PS_BRUTO,
                    UnidadesPorPallet = ppp.ProductoPallet.QT_UNIDADES
                })
                .FirstOrDefault();
        }

        protected virtual SugerenciaAlmacenamiento GetUltimaSugerenciaNoAprobada(CriterioAlmacenaje criterio, AsociacionEstrategia asociacion)
        {
            var clase = asociacion.Clase ?? "*";
            var grupo = asociacion.Grupo ?? "*";
            var producto = asociacion.Producto ?? "*";

            var sugerencia = _context.T_ALM_SUGERENCIA
                .Include("T_ALM_LOGICA_INSTANCIA")
                .AsNoTracking()
                .Where(s => s.NU_ALM_ESTRATEGIA == asociacion.Estrategia
                    && s.CD_ALM_OPERATIVA_ASOCIABLE == asociacion.Operativa.Codigo
                    && s.NU_PREDIO == asociacion.Predio
                    && s.CD_CLASSE == clase
                    && s.CD_GRUPO == grupo
                    && s.CD_EMPRESA == (asociacion.Empresa ?? -1)
                    && s.CD_PRODUTO == producto
                    && s.CD_REFERENCIA == criterio.Referencia
                    && s.CD_AGRUPADOR == criterio.Agrupador.Id
                    && s.TP_ALM_OPERATIVA_ASOCIABLE == asociacion.Operativa.Tipo
                    && s.CD_ESTADO == AlmacenamientoDb.ESTADO_APROBADO)
                 .OrderByDescending(s => s.DT_ADDROW)
                 .FirstOrDefault();

            return _mapper.Map(sugerencia);
        }

        protected virtual SugerenciaAlmacenamiento GetUltimaSugerenciaPendiente(CriterioAlmacenaje criterio)
        {

            var sugerencia = _context.T_ALM_SUGERENCIA
                .Include("T_ALM_LOGICA_INSTANCIA")
                .AsNoTracking()
                .Where(s => s.CD_REFERENCIA == criterio.Referencia
                    && s.CD_AGRUPADOR == criterio.Agrupador.Id
                    && s.TP_ALM_OPERATIVA_ASOCIABLE == criterio.Operativa.Tipo
                    && s.CD_ESTADO == AlmacenamientoDb.ESTADO_PENDIENTE)
                 .OrderByDescending(s => s.DT_ADDROW)
                 .FirstOrDefault();

            return _mapper.Map(sugerencia);
        }

        protected virtual string GetUbicacionInicial(string nuPredio, bool ordenAsc)
        {
            var ubicaciones = _context.T_ENDERECO_ESTOQUE
                .AsNoTracking()
                .Where(e => e.NU_PREDIO == nuPredio)
                .Select(e => e.CD_ENDERECO);

            if (ordenAsc)
                return ubicaciones.Min();
            else
                return ubicaciones.Max();
        }

        protected virtual T_ALM_SUGERENCIA SugerirUbicacion(CriterioAlmacenaje criterio, DateTime inicioCalculo, AsociacionEstrategia asociacion, InstanciaLogica logica, SugerenciaAlmacenamiento ultimaSugerencia, long nuTransaccionDB, int nuEtiqueta)
        {
            var sugerencia = null as T_ALM_SUGERENCIA;
            var ordenAsc = logica.OrdenarAscendente == "S";
            var ubicacionInicio = ultimaSugerencia?.UbicacionSugerida;
            var incluirUbicacionInicio = ultimaSugerencia == null;
            var ubicacionesSql = GetFiltrarUbicacionesSql(criterio, ordenAsc, ubicacionInicio, incluirUbicacionInicio, asociacion, logica, nuEtiqueta, out Dictionary<string, object> parametrosSql);
            var eo = new ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

            ubicacionesSql = OrdenarUbicaciones(ordenAsc, incluirUbicacionInicio, ubicacionesSql, logica, ubicacionInicio, asociacion.Predio, parametrosSql);

            foreach (var kvp in parametrosSql)
            {
                eoColl.Add(kvp);
            }

            dynamic eoDynamic = eo;

            var ubicaciones = (IEnumerable<Ubicacion>)_dapper.Query<Ubicacion>(
                _context.Database.GetDbConnection(),
                ubicacionesSql,
                eoDynamic,
                transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            Ubicacion ubicacion = null;

            var firstProducto = criterio.Productos.FirstOrDefault();
            var minProducto = criterio.Productos.Min(x => x.Codigo);
            var maxProducto = criterio.Productos.Max(x => x.Codigo);

            foreach (var ubic in ubicaciones)
            {
                var tpUbicacion = _context.T_TIPO_ENDERECO.FirstOrDefault(x => x.CD_TIPO_ENDERECO == ubic.IdUbicacionTipo);

                if (tpUbicacion.ID_VARIOS_PRODUTOS == "N"
                    && _context.T_STOCK
                        .AsNoTracking()
                        .Any(s => s.CD_ENDERECO == ubic.Id && (s.CD_EMPRESA != firstProducto.Empresa || s.CD_PRODUTO != minProducto || s.CD_PRODUTO != maxProducto)))
                {
                    continue;
                }
                else if (tpUbicacion.ID_VARIOS_LOTES == "N"
                        && _context.T_STOCK
                            .Join(_context.T_ALM_DET_PRODUTO_TEMP,
                                s => new { s.CD_EMPRESA, s.CD_PRODUTO },
                                alm => new { alm.CD_EMPRESA, alm.CD_PRODUTO },
                                (s, alm) => new { Stock = s, AlmDetalleTemp = alm })
                            .AsNoTracking()
                            .Any(s => s.AlmDetalleTemp.NU_ETIQUETA_LOTE == nuEtiqueta
                                && s.AlmDetalleTemp.USERID == _userId
                                && s.Stock.CD_ENDERECO == ubic.Id
                                && s.Stock.CD_EMPRESA == firstProducto.Empresa
                                && s.Stock.NU_IDENTIFICADOR != s.AlmDetalleTemp.NU_IDENTIFICADOR))
                {
                    continue;
                }
                else
                {
                    ubicacion = ubic;
                    break;
                }
            }

            if (ubicacion != null)
            {
                var now = DateTime.Now;

                sugerencia = new T_ALM_SUGERENCIA
                {
                    NU_ALM_SUGERENCIA = _context.GetNextSequenceValueLong(_dapper, "S_NU_ALM_SUGERENCIA"),
                    CD_AGRUPADOR = criterio.Agrupador.Id,
                    CD_ALM_OPERATIVA_ASOCIABLE = asociacion.Operativa.Codigo,
                    CD_CLASSE = asociacion.Clase ?? "*",
                    CD_EMPRESA = asociacion.Empresa ?? -1,
                    CD_ENDERECO_SUGERIDO = ubicacion.Id,
                    CD_ESTADO = AlmacenamientoDb.ESTADO_PENDIENTE,
                    CD_FUNCIONARIO = _userId,
                    CD_GRUPO = asociacion.Grupo ?? "*",
                    CD_PRODUTO = asociacion.Producto ?? "*",
                    CD_REFERENCIA = criterio.Referencia,
                    DT_ADDROW = now,
                    DT_UPDROW = now,
                    NU_ALM_ESTRATEGIA = asociacion.Estrategia,
                    NU_ALM_LOGICA_INSTANCIA = logica.Id,
                    NU_PREDIO = asociacion.Predio,
                    NU_TRANSACCION = nuTransaccionDB,
                    TP_ALM_OPERATIVA_ASOCIABLE = asociacion.Operativa.Tipo,
                    VL_TIEMPO_CALCULO = Convert.ToDecimal((DateTime.Now - inicioCalculo).TotalMinutes),
                    CD_EMPRESA_AGRUPADOR = firstProducto.Empresa
                };
            }

            return sugerencia;
        }

        public virtual string OrdenarUbicaciones(bool ordenAsc, bool incluirUbicacionInicio, string ubicacionesSql, InstanciaLogica logica, string ubicacionInicio, string predio, Dictionary<string, object> parametrosSql)
        {

            var ubicInicio = GetParametro(logica.Parametros, "UBIC_INICIO");

            if (string.IsNullOrEmpty(ubicacionInicio))
                ubicacionInicio = ubicInicio;

            if (string.IsNullOrEmpty(ubicacionInicio))
                ubicacionInicio = GetUbicacionInicial(predio, ordenAsc);


            parametrosSql["UBIC_INICIO"] = ubicacionInicio;

            ubicacionesSql = @$"
                SELECT 
                    ee.CD_ENDERECO as Id,
                    ee.CD_TIPO_ENDERECO as IdUbicacionTipo
                FROM ({ubicacionesSql}) ee
                WHERE EE.CD_ENDERECO {(ordenAsc ? (incluirUbicacionInicio ? ">=" : ">") : (incluirUbicacionInicio ? "<=" : "<"))} :UBIC_INICIO
                ORDER BY ee.CD_ENDERECO {(ordenAsc ? "ASC" : "DESC")}";

            return ubicacionesSql;
        }

        protected virtual string GetFiltrarUbicacionesSql(CriterioAlmacenaje criterio, bool ordenAsc, string ubicacionInicio, bool incluirUbicacionInicio, AsociacionEstrategia asociacion, InstanciaLogica logica, int nuEtiqueta, out Dictionary<string, object> parametrosSql)
        {
            parametrosSql = new Dictionary<string, object>();

            var ubicacionesSql = GetFiltrarUbicacionesPorParametrosSql(criterio, ordenAsc, ubicacionInicio, incluirUbicacionInicio, asociacion, logica, parametrosSql, nuEtiqueta, out decimal? porcentajeOcupacion, out int porcentajeParcializacion);

            ubicacionesSql = GetFiltrarUbicacionesPorLogicaSql(criterio, logica, ubicacionesSql);

            if (logica.Logica != AlmacenamientoDb.LOGICA_REABASTECIMIENTOS)
            {
                ubicacionesSql = GetFiltrarUbicacionesPorVolumenSql(criterio, logica, porcentajeOcupacion, ubicacionesSql, parametrosSql);
                ubicacionesSql = GetFiltrarUbicacionesPorDimensionesSql(criterio, ubicacionesSql, parametrosSql);
                ubicacionesSql = GetFiltrarUbicacionesPorPesoSql(criterio, logica, ubicacionesSql, parametrosSql);
            }

            return ubicacionesSql;
        }

        protected virtual string GetFiltrarUbicacionesPorParametrosSql(CriterioAlmacenaje criterio, bool ordenAsc, string ubicacionInicio, bool incluirUbicacionInicio, AsociacionEstrategia asociacion, InstanciaLogica logica, Dictionary<string, object> parametrosSql, int nuEtiqueta, out decimal? porcentajeOcupacion, out int porcentajeParcializacion)
        {
            var parametros = logica.Parametros;

            var pallet = GetParametroShort(parametros, "PALLET");

            var filtroUbicaciones = "TA.ID_PERMITE_ALMACENAR = 'S' AND TA.ID_AREA_PICKING = 'N'";
            if (logica.Logica == AlmacenamientoDb.LOGICA_REABASTECIMIENTOS)
            {
                filtroUbicaciones = "TA.ID_AREA_PICKING = 'S'";
            }

            //En caso de tomarse en cuenta ubicaciones de picking de automatismo - GET_ENDERECO_ENTRADA_AUTOMATISMO
            var ubicacionesSql = @$"
                    SELECT   
                        COALESCE(EEA.CD_ENDERECO_ENTRADA, EE.CD_ENDERECO) CD_ENDERECO,
                        EE.CD_EMPRESA,
                        EE.CD_TIPO_ENDERECO,
                        EE.CD_ROTATIVIDADE,
                        EE.CD_FAMILIA_PRINCIPAL,
                        EE.CD_CLASSE,
                        EE.CD_SITUACAO,
                        EE.ID_ENDERECO_BAIXO,
                        EE.ID_ENDERECO_SEP,
                        EE.ID_NECESSIDADE_RESUPRIR,
                        EE.DT_UPDROW,
                        EE.DT_ADDROW,
                        EE.CD_CONTROL,
                        EE.CD_AREA_ARMAZ,
                        EE.NU_COMPONENTE,
                        EE.CD_ZONA_UBICACION,
                        EE.NU_PREDIO,
                        EE.ID_BLOQUE,
                        EE.ID_CALLE,
                        EE.NU_COLUMNA,
                        EE.NU_ALTURA,
                        EE.ND_SECTOR,
                        EE.CD_CONTROL_ACCESO,
                        EE.NU_PROFUNDIDAD,
                        EE.CD_BARRAS,
                        EE.NU_ORDEN_DEFAULT
                    FROM T_ENDERECO_ESTOQUE EE
                    INNER JOIN T_TIPO_ENDERECO TE ON TE.CD_TIPO_ENDERECO = EE.CD_TIPO_ENDERECO
                    INNER JOIN T_TIPO_AREA TA ON TA.CD_AREA_ARMAZ = EE.CD_AREA_ARMAZ
                    {(pallet.HasValue ? "LEFT JOIN T_TIPO_ENDERECO_PALLET TEP ON TEP.CD_TIPO_ENDERECO = TE.CD_TIPO_ENDERECO" : "")}
                    LEFT JOIN V_ENDERECO_ENTRADA_AUTOMATISMO EEA ON EEA.CD_ENDERECO_AUTOMATISMO = EE.CD_ENDERECO
                    WHERE EE.NU_PREDIO = :PREDIO
                        AND {filtroUbicaciones} 
                        AND EE.CD_CLASSE = :CLASE
                ";

            parametrosSql["PREDIO"] = asociacion.Predio;
            parametrosSql["CLASE"] = criterio.Clase;

            var alturaDesde = GetParametroInt(parametros, "ALTURA_DESDE");
            if (alturaDesde.HasValue)
            {
                var alturaHasta = GetParametroInt(parametros, "ALTURA_HASTA");
                ubicacionesSql += @"
                    AND ee.NU_ALTURA IS NOT NULL
                    AND :ALTURA_DESDE <= ee.NU_ALTURA
                    AND ee.NU_ALTURA <= :ALTURA_HASTA
                ";

                parametrosSql["ALTURA_DESDE"] = alturaDesde.Value;
                parametrosSql["ALTURA_HASTA"] = alturaHasta.Value;
            }

            var area = GetParametroShort(parametros, "AREA");
            if (area.HasValue)
            {
                ubicacionesSql += @"
                        AND ee.CD_AREA_ARMAZ = :AREA
                    ";
                parametrosSql["AREA"] = area.Value;
            }

            var bloqueDesde = GetParametro(parametros, "BLOQUE_DESDE");
            if (!string.IsNullOrEmpty(bloqueDesde))
            {
                var bloqueHasta = GetParametro(parametros, "BLOQUE_HASTA");
                ubicacionesSql += @"
                        AND ee.ID_BLOQUE IS NOT NULL
                        AND :BLOQUE_DESDE <= ee.ID_BLOQUE
                        AND ee.ID_BLOQUE <= :BLOQUE_HASTA
                    ";
                parametrosSql["BLOQUE_DESDE"] = bloqueDesde;
                parametrosSql["BLOQUE_HASTA"] = bloqueHasta;
            }

            var calleDesde = GetParametro(parametros, "CALLE_DESDE");
            if (!string.IsNullOrEmpty(calleDesde))
            {
                var calleHasta = GetParametro(parametros, "CALLE_HASTA");
                ubicacionesSql += @"
                        AND ee.ID_CALLE IS NOT NULL
                        AND :CALLE_DESDE <= ee.ID_CALLE
                        AND ee.ID_CALLE <= :CALLE_HASTA
                    ";
                parametrosSql["CALLE_DESDE"] = calleDesde;
                parametrosSql["CALLE_HASTA"] = calleHasta;
            }

            var columnaDesde = GetParametroInt(parametros, "COLUMNA_DESDE");
            if (columnaDesde.HasValue)
            {
                var columnaHasta = GetParametroInt(parametros, "COLUMNA_HASTA");
                ubicacionesSql += @"
                        AND ee.NU_COLUMNA IS NOT NULL
                        AND :COLUMNA_DESDE <= ee.NU_COLUMNA
                        AND ee.NU_COLUMNA <= :COLUMNA_HASTA
                    ";
                parametrosSql["COLUMNA_DESDE"] = columnaDesde;
                parametrosSql["COLUMNA_HASTA"] = columnaHasta;
            }

            var empresa = GetParametroInt(parametros, "EMPRESA");
            if (empresa.HasValue)
            {
                ubicacionesSql += @"
                        AND ee.CD_EMPRESA = :EMPRESA
                    ";
                parametrosSql["EMPRESA"] = empresa.Value;
            }

            var familia = GetParametroInt(parametros, "FAMILIA");
            if (familia.HasValue)
            {
                ubicacionesSql += @"
                        AND ee.CD_FAMILIA_PRINCIPAL = :FAMILIA
                    ";
                parametrosSql["FAMILIA"] = familia.Value;
            }

            if (pallet.HasValue)
            {
                ubicacionesSql += @"
                        AND tep.CD_PALLET = :PALLET
                    ";
                parametrosSql["PALLET"] = pallet.Value;
            }

            var rotatividad = GetParametroShort(parametros, "ROTATIVIDAD");
            if (rotatividad.HasValue)
            {
                ubicacionesSql += @"
                        AND ee.CD_ROTATIVIDADE = :ROTATIVIDAD
                    ";
                parametrosSql["ROTATIVIDAD"] = rotatividad.Value;
            }

            var tipoUbicacion = GetParametroShort(parametros, "TIPO_UBICACION");
            if (tipoUbicacion.HasValue)
            {
                ubicacionesSql += @"
                        AND ee.CD_TIPO_ENDERECO = :TIPO_UBICACION
                    ";
                parametrosSql["TIPO_UBICACION"] = tipoUbicacion.Value;
            }

            var ubicBajasAltas = GetParametro(parametros, "UBIC_BAJAS_ALTAS");
            if (!string.IsNullOrEmpty(ubicBajasAltas))
            {
                ubicacionesSql += @"
                        AND ee.ID_ENDERECO_BAIXO IS NOT NULL
                        AND ee.ID_ENDERECO_BAIXO = :UBIC_BAJAS_ALTAS
                    ";
                parametrosSql["UBIC_BAJAS_ALTAS"] = ubicBajasAltas;
            }

            var cantidadProductosEtiqueta = criterio.Productos.GroupBy(p => p.Codigo).Count();

            var ubicMultiproducto = GetParametro(parametros, "UBIC_MULTIPRODUCTO");
            if (!string.IsNullOrEmpty(ubicMultiproducto))
            {
                ubicacionesSql += @"
                        AND te.ID_VARIOS_PRODUTOS = :UBIC_MULTIPRODUCTO
                    ";
                parametrosSql["UBIC_MULTIPRODUCTO"] = ubicMultiproducto;

                if (ubicMultiproducto == "N" && cantidadProductosEtiqueta > 1)
                {
                    ubicacionesSql += @"
                        AND 1 <> 1
                    ";
                }
            }


            var ubicMultilote = GetParametro(parametros, "UBIC_MULTILOTE");
            if (!string.IsNullOrEmpty(ubicMultilote))
            {
                ubicacionesSql += @"
                        AND te.ID_VARIOS_LOTES = :UBIC_MULTILOTE
                    ";
                parametrosSql["UBIC_MULTILOTE"] = ubicMultilote;

                if (ubicMultilote == "N" && criterio.Productos.GroupBy(p => new { p.Codigo }).Any(g => g.Select(x => x.Lote).Distinct().Count() > 1))
                {
                    ubicacionesSql += @"
                        AND 1 <> 1
                    ";
                }
            }

            var zona = GetParametro(parametros, "ZONA_UBICACION");
            if (!string.IsNullOrEmpty(zona))
            {
                ubicacionesSql += @"
                        AND ee.CD_ZONA_UBICACION IS NOT NULL
                        AND ee.CD_ZONA_UBICACION = :ZONA_UBICACION
                    ";
                parametrosSql["ZONA_UBICACION"] = zona;
            }

            var controlAcceso = GetParametro(parametros, "CONTROL_ACCESO");
            if (!string.IsNullOrEmpty(controlAcceso))
            {
                ubicacionesSql += @"
                        AND ee.CD_CONTROL_ACCESO IS NOT NULL
                        AND ee.CD_CONTROL_ACCESO = :CONTROL_ACCESO
                    ";
                parametrosSql["CONTROL_ACCESO"] = controlAcceso;
            }

            porcentajeOcupacion = GetParametroDecimal(parametros, "PORCENTAJE_OCUPACION");
            if (!porcentajeOcupacion.HasValue)
            {
                porcentajeOcupacion = 100;
            }

            var respetaVencimiento = GetParametro(parametros, "RESPETA_VENCIMIENTO");
            if ((respetaVencimiento ?? "N") == "S") // Parámetro exclusivo para Lógicas de Remonte
            {
                var ubicacionesInvalidasSql = @$"
                    SELECT s.CD_ENDERECO
                    FROM T_ALM_DET_PRODUTO_TEMP temp
                    INNER JOIN T_PRODUTO p on temp.CD_PRODUTO = p.CD_PRODUTO AND temp.CD_EMPRESA = p.CD_EMPRESA
                    INNER JOIN T_STOCK s ON  temp.CD_PRODUTO = s.CD_PRODUTO AND
                        temp.CD_FAIXA = s.CD_FAIXA AND
                        temp.NU_IDENTIFICADOR = s.NU_IDENTIFICADOR AND
                        temp.CD_EMPRESA = s.CD_EMPRESA
                    WHERE p.TP_MANEJO_FECHA = 'E' AND s.DT_FABRICACAO <> temp.DT_FABRICACAO
                    GROUP BY s.CD_ENDERECO";

                ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        LEFT JOIN ({ubicacionesInvalidasSql}) s ON ee.CD_ENDERECO = s.CD_ENDERECO
                        WHERE s.CD_ENDERECO IS NULL
                    ";
            }

            var respetaFifo = GetParametro(parametros, "RESPETA_FIFO");
            if ((respetaFifo ?? "N") == "S") // Parámetro exclusivo para Lógicas de Reabastecimientos
            {
                var ubicacionesInvalidasSql = @$"
                    SELECT s.CD_ENDERECO
                    FROM T_ALM_DET_PRODUTO_TEMP temp
                    INNER JOIN T_PRODUTO p on temp.CD_PRODUTO = p.CD_PRODUTO AND temp.CD_EMPRESA = p.CD_EMPRESA
                    INNER JOIN T_STOCK s ON  temp.CD_PRODUTO = s.CD_PRODUTO AND
                        temp.CD_FAIXA = s.CD_FAIXA AND
                        temp.NU_IDENTIFICADOR = s.NU_IDENTIFICADOR AND
                        temp.CD_EMPRESA = s.CD_EMPRESA
                    WHERE p.TP_MANEJO_FECHA = 'F' AND s.DT_FABRICACAO <> temp.DT_FABRICACAO
                    GROUP BY s.CD_ENDERECO";

                ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        LEFT JOIN ({ubicacionesInvalidasSql}) s ON ee.CD_ENDERECO = s.CD_ENDERECO
                        WHERE s.CD_ENDERECO IS NULL
                    ";
            }

            var respetaLote = GetParametro(parametros, "RESPETA_LOTE");
            var lotesCoincidentes = GetParametro(parametros, "LOTES_COINCIDENTES");
            if ((respetaLote ?? "N") == "S" || (lotesCoincidentes ?? "N") == "S")
            {
                var ubicacionesMismoLoteSql = @$"
                    SELECT 
                        s.CD_ENDERECO,
                        COUNT(*) QT_LOTE
                    FROM T_ALM_DET_PRODUTO_TEMP temp
                    INNER JOIN T_STOCK s ON  temp.CD_PRODUTO = s.CD_PRODUTO AND
                        temp.CD_FAIXA = s.CD_FAIXA AND
                        temp.NU_IDENTIFICADOR = s.NU_IDENTIFICADOR AND
                        temp.CD_EMPRESA = s.CD_EMPRESA
                    WHERE COALESCE(s.QT_ESTOQUE, 0) > 0
                    GROUP BY s.CD_ENDERECO";

                var cantidadProdutoLoteEtiqueta = criterio.Productos.GroupBy(p => new { p.Codigo, p.Lote }).Count();

                parametrosSql["QT_PRODUTO_LOTE"] = cantidadProdutoLoteEtiqueta;

                ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        INNER JOIN ({ubicacionesMismoLoteSql}) s ON ee.CD_ENDERECO = s.CD_ENDERECO AND s.QT_LOTE = :QT_PRODUTO_LOTE";
            }

            var productosCoincidentes = GetParametro(parametros, "PRODUCTOS_COINCIDENTES");
            if ((productosCoincidentes ?? "N") == "S")
            {
                var ubicacionesProductoSql = @$"
                        SELECT s.CD_ENDERECO
                        FROM T_ENDERECO_ESTOQUE ee
                        INNER JOIN T_STOCK s ON ee.CD_ENDERECO = s.CD_ENDERECO
                        INNER JOIN T_ALM_DET_PRODUTO_TEMP temp   ON  temp.CD_PRODUTO = s.CD_PRODUTO AND temp.CD_EMPRESA = s.CD_EMPRESA
                        GROUP BY s.CD_ENDERECO";

                ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        INNER JOIN ({ubicacionesProductoSql}) s ON ee.CD_ENDERECO = s.CD_ENDERECO
                    ";
            }


            if (logica.Logica == AlmacenamientoDb.LOGICA_REABASTECIMIENTOS)
            {
                parametrosSql["FECHA_HOY"] = DateTime.Now.Date;

                var ventanaFefoParam = GetParametroDecimal(parametros, "VENTANA_FEFO");
                if (ventanaFefoParam.HasValue)
                {
                    parametrosSql["VENTANA_FEFO"] = ventanaFefoParam;
                }
                else
                    parametrosSql["VENTANA_FEFO"] = 0;

                var stockProductoEtiquetaSql = @$"
                     SELECT 
                        NU_ETIQUETA_LOTE,
                        CD_PRODUTO,
                        SUM(QT_ESTOQUE) QT_ESTOQUE
                     FROM T_ALM_DET_PRODUTO_TEMP
                     GROUP BY 
                        NU_ETIQUETA_LOTE,
                        CD_PRODUTO";

                var ubicacionesInvalidasSql = @$"
                    SELECT upr.CD_ENDERECO_PI AS CD_ENDERECO
                    FROM T_ALM_DET_PRODUTO_TEMP temp
                    INNER JOIN ({stockProductoEtiquetaSql}) spe ON TEMP.NU_ETIQUETA_LOTE = spe.NU_ETIQUETA_LOTE
                        AND TEMP.CD_PRODUTO = spe.CD_PRODUTO
                    INNER JOIN V_UBIC_PICKING_REAB upr ON temp.CD_PRODUTO = upr.CD_PRODUTO
                        AND temp.CD_EMPRESA = upr.CD_EMPRESA
                    WHERE (upr.TP_MANEJO_FECHA = 'E' AND :IGNORAR_VENCIMIENTO_STOCK = 'N' AND upr.DT_FABRICACAO is not null AND DATEADD('DAY', :VENTANA_FEFO, COALESCE(upr.DT_FABRICACAO, :FECHA_HOY)) < temp.DT_FABRICACAO)
                        OR MOD(spe.QT_ESTOQUE, upr.QT_PADRAO_PI) <> 0 
                        OR (spe.QT_ESTOQUE + upr.QT_STOCK_PI) > upr.QT_MAXIMO_PI
                        OR (:PORCENTAJE_FORZADO <> -1 AND (upr.QT_MINIMO_PI * (1+(:PORCENTAJE_FORZADO/100))) <= upr.QT_STOCK_PI) 
                    GROUP BY upr.CD_ENDERECO_PI";


                var filtroTipoPicking = "";

                var tipoPicking = GetParametro(parametros, "TIPO_PICKING");
                if (!string.IsNullOrEmpty(tipoPicking))
                {
                    filtroTipoPicking += @$" AND upr.TP_PICKING = :TP_PICKING ";
                    parametrosSql["TP_PICKING"] = tipoPicking;
                }

                var cantidadProductosUbicacion = @$"
                    SELECT 
                        upr.CD_ENDERECO_PI AS CD_ENDERECO,
                        COUNT(*) QT_PRODUTO_ASOCIADO
                    FROM T_ALM_DET_PRODUTO_TEMP temp
                    INNER JOIN V_UBIC_PICKING_REAB upr ON temp.CD_PRODUTO = upr.CD_PRODUTO
                        AND temp.CD_EMPRESA = upr.CD_EMPRESA {filtroTipoPicking}
                    GROUP BY upr.CD_ENDERECO_PI";

                var ubicacionesProductoSql = @$"
                    SELECT 
                        upr.CD_ENDERECO_PI AS CD_ENDERECO,
                        MIN(upr.QT_PADRAO_PI) MIN_QT_PADRAO_PICKING,
                        MAX(upr.QT_PADRAO_PI) MAX_QT_PADRAO_PICKING,                                        
                        MIN(upr.ID_MENOS_MINIMO_PI) MIN_ID_MENOS_MINIMO_PI,
                        MAX(upr.ID_MENOS_MINIMO_PI) MAX_ID_MENOS_MINIMO_PI,
                        MIN(upr.ID_MENOS_PEDIDO_PI) MIN_ID_MENOS_PEDIDO_PI,
                        MAX(upr.ID_MENOS_PEDIDO_PI) MAX_ID_MENOS_PEDIDO_PI
                    FROM T_ALM_DET_PRODUTO_TEMP temp
                    INNER JOIN V_UBIC_PICKING_REAB upr ON temp.CD_PRODUTO = upr.CD_PRODUTO
                        AND temp.CD_EMPRESA = upr.CD_EMPRESA
                    INNER JOIN ({cantidadProductosUbicacion}) cpu ON cpu.CD_ENDERECO = upr.CD_ENDERECO_PI
                    LEFT JOIN ({ubicacionesInvalidasSql}) ui ON ui.CD_ENDERECO = upr.CD_ENDERECO_PI
                    WHERE ui.CD_ENDERECO IS NULL AND :QT_PRODUTO_ETIQUETA = cpu.QT_PRODUTO_ASOCIADO
                    GROUP BY upr.CD_ENDERECO_PI";

                var ignorarVencimiengoSotck = GetParametro(parametros, "IGNORAR_VENCIMIENTO_STOCK");

                if (!string.IsNullOrEmpty(ignorarVencimiengoSotck))
                {
                    parametrosSql["IGNORAR_VENCIMIENTO_STOCK"] = ignorarVencimiengoSotck;
                }
                else
                {
                    parametrosSql["IGNORAR_VENCIMIENTO_STOCK"] = "N";
                }

                parametrosSql["QT_PRODUTO_ETIQUETA"] = cantidadProductosEtiqueta;

                ubicacionesSql = @$"
                            SELECT ee.*
                            FROM ({ubicacionesSql}) ee
                            INNER JOIN ({ubicacionesProductoSql}) s ON ee.CD_ENDERECO = s.CD_ENDERECO";


                var modalidadReabastecimiento = GetParametro(parametros, "MODALIDAD_REABASTECIMIENTO");

                if (!string.IsNullOrEmpty(modalidadReabastecimiento))
                {
                    parametrosSql["PORCENTAJE_FORZADO"] = -1;
                    switch (modalidadReabastecimiento)
                    {
                        case EstrategiaAlmacenajeReabastecimientoDb.MINIMO:
                            ubicacionesSql += @$" AND s.MIN_ID_MENOS_MINIMO_PI = 'S' AND s.MAX_ID_MENOS_MINIMO_PI  = 'S' ";
                            break;
                        case EstrategiaAlmacenajeReabastecimientoDb.URGENTE:
                            ubicacionesSql += @$" AND s.MIN_ID_MENOS_PEDIDO_PI = 'S' AND s.MAX_ID_MENOS_PEDIDO_PI  = 'S' ";
                            break;
                        case EstrategiaAlmacenajeReabastecimientoDb.FORZADO:
                            var porcForzadoParam = GetParametroDecimal(parametros, "PORCENTAJE_FORZADO");
                            if (porcForzadoParam.HasValue)
                            {
                                parametrosSql["PORCENTAJE_FORZADO"] = porcForzadoParam;
                            }
                            break;
                    }
                }

                var padron = GetParametro(parametros, "PADRON");
                if (!string.IsNullOrEmpty(padron))
                {
                    ubicacionesSql += @$" AND s.MIN_QT_PADRAO_PICKING = :PADRON AND s.MAX_QT_PADRAO_PICKING = :PADRON ";

                    parametrosSql["PADRON"] = padron;
                }
            }

            porcentajeParcializacion = 100;
            if (criterio.Productos.Count() == 1)
            {
                var porcentajeParcializacionParam = GetParametro(parametros, "PORCENTAJE_PARCIALIZACION");
                int.TryParse(porcentajeParcializacionParam, out porcentajeParcializacion);
            }



            return ubicacionesSql;
        }

        protected virtual string GetFiltrarUbicacionesPorLogicaSql(CriterioAlmacenaje criterio, InstanciaLogica logica, string ubicacionesSql)
        {
            if (logica.Logica == AlmacenamientoDb.LOGICA_UBICACIONES_VACIAS || logica.Logica == AlmacenamientoDb.LOGICA_REMONTE_UBICACIONES)
            {
                var ubicacionesStockSql = @"
                    SELECT 
                        s.CD_ENDERECO,
                        SUM (COALESCE(s.QT_ESTOQUE,0) + COALESCE(s.QT_TRANSITO_ENTRADA,0)) AS QT_ESTOQUE
                    FROM T_ENDERECO_ESTOQUE ee
                    INNER JOIN T_STOCK s ON s.CD_ENDERECO = ee.CD_ENDERECO
                    GROUP BY s.CD_ENDERECO
                ";

                ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        LEFT JOIN ({ubicacionesStockSql}) s ON ee.CD_ENDERECO = s.CD_ENDERECO
                        WHERE COALESCE(s.QT_ESTOQUE,0) {((logica.Logica == AlmacenamientoDb.LOGICA_UBICACIONES_VACIAS) ? "<=" : ">")} 0
                    ";
            }

            return ubicacionesSql;
        }

        protected virtual string GetFiltrarUbicacionesPorVolumenSql(CriterioAlmacenaje criterio, InstanciaLogica logica, decimal? porcentajeOcupacion, string ubicacionesSql, Dictionary<string, object> parametrosSql)
        {
            return GetFiltrarUbicacionesPorVolumetriaSql(criterio, logica, porcentajeOcupacion, ubicacionesSql, parametrosSql);
        }

        protected virtual string GetFiltrarUbicacionesPorCapacidadDeUnidadesSql(CriterioAlmacenaje criterio, InstanciaLogica logica, string ubicacionesSql, Dictionary<string, object> parametrosSql)
        {
            if (criterio.Pallet.HasValue && criterio.Productos.GroupBy(x => x.Codigo).Count() == 1)
            {
                var palletsOcupadosPorProductoSql = @"
                        SELECT 
                            s.CD_ENDERECO,
                            s.CD_PRODUTO,
                            s.CD_EMPRESA,
                            SUM (COALESCE(s.QT_ESTOQUE, 0) + COALESCE(s.QT_TRANSITO_ENTRADA, 0)) AS QT_ESTOQUE,
                            COALESCE(pp.QT_UNIDADES, 0) AS QT_UNIDADES_PALLET,
                            COALESCE(tep.QT_PALLET, 0) AS QT_PALLET
                        FROM T_ENDERECO_ESTOQUE ee
                        LEFT JOIN T_STOCK s ON s.CD_ENDERECO = ee.CD_ENDERECO
                        LEFT JOIN T_PRODUTO_PALLET pp ON pp.CD_EMPRESA = s.CD_EMPRESA
                            AND pp.CD_PRODUTO = s.CD_PRODUTO
                            AND pp.CD_FAIXA = s.CD_FAIXA
                            AND pp.CD_PALLET = :CRITERIO_PALLET 
                        LEFT JOIN T_TIPO_ENDERECO_PALLET tep ON tep.CD_TIPO_ENDERECO = ee.CD_TIPO_ENDERECO 
                            AND tep.CD_PALLET = :CRITERIO_PALLET
                        GROUP BY 
                            s.CD_ENDERECO,
                            s.CD_PRODUTO,
                            s.CD_EMPRESA,
                            pp.QT_UNIDADES,
                            tep.QT_PALLET
                    ";

                var palletsOcupadosSql = @$"
                        SELECT 
                            pop.CD_ENDERECO,
                            CASE WHEN MIN(pop.QT_UNIDADES_PALLET) = 0 OR MIN(pop.QT_PALLET) = 0 THEN -1 ELSE SUM (pop.QT_UNIDADES_PALLET / pop.QT_PALLET) END AS QT_PALLET
                        FROM ({palletsOcupadosPorProductoSql}) pop
                        GROUP BY pop.CD_ENDERECO
                    ";

                var palletsPorUbicacionSql = @"
                        SELECT 
                            ee.CD_ENDERECO,
                            COALESCE(tep.QT_PALLET, -1) AS QT_PALLET
                        FROM T_ENDERECO_ESTOQUE ee
                        LEFT JOIN T_TIPO_ENDERECO_PALLET tep ON tep.CD_TIPO_ENDERECO = ee.CD_TIPO_ENDERECO 
                            AND tep.CD_PALLET = :CRITERIO_PALLET
                    ";

                parametrosSql["CRITERIO_PALLET"] = criterio.Pallet.Value;

                var cantidadProducto = criterio.Productos.Sum(x => x.CantidadSeparar);
                var cantidadPallets = 0m;
                var unidadesPorPallet = criterio.Productos.FirstOrDefault().UnidadesPorPallet ?? 0;

                if (unidadesPorPallet > 0)
                {
                    cantidadPallets = cantidadProducto / unidadesPorPallet;
                }

                if (cantidadPallets > 0)
                {
                    parametrosSql["CANTIDAD_PALLETS"] = cantidadPallets;

                    if (logica.Logica == AlmacenamientoDb.LOGICA_UBICACIONES_VACIAS)
                    {
                        palletsPorUbicacionSql += @"
                            WHERE tep.QT_PALLET = -1 
                                OR :CANTIDAD_PALLETS <= tep.QT_PALLET
                        ";
                    }
                    else
                    {
                        palletsPorUbicacionSql += @$"
                            INNER JOIN ({palletsOcupadosSql}) po ON po.CD_ENDERECO = ee.CD_ENDERECO
                            WHERE po.QT_PALLET = -1 
                                OR tep.QT_PALLET = -1 
                                OR :CANTIDAD_PALLETS <= tep.QT_PALLET - po.QT_PALLET
                        ";
                    }

                    ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        INNER JOIN ({palletsPorUbicacionSql}) ppu ON ppu.CD_ENDERECO = ee.CD_ENDERECO
                    ";
                }
            }

            return ubicacionesSql;
        }

        protected virtual string GetFiltrarUbicacionesPorVolumetriaSql(CriterioAlmacenaje criterio, InstanciaLogica logica, decimal? porcentajeOcupacion, string ubicacionesSql, Dictionary<string, object> parametrosSql)
        {
            var volumenOcupadoSql = @"
                SELECT 
                    s.CD_ENDERECO,
                    SUM ((COALESCE(s.QT_ESTOQUE, 0) + COALESCE(s.QT_TRANSITO_ENTRADA, 0)) * COALESCE(p.VL_CUBAGEM, 0)) AS VL_CUBAGEM,
                    SUM (CASE WHEN p.VL_CUBAGEM IS NULL THEN 1 ELSE 0 END) AS QT_NULL_CUBAGEM
                FROM T_ENDERECO_ESTOQUE ee
                LEFT JOIN T_STOCK s ON s.CD_ENDERECO = ee.CD_ENDERECO
                INNER JOIN T_PRODUTO p ON p.CD_EMPRESA = s.CD_EMPRESA
                    AND p.CD_PRODUTO = s.CD_PRODUTO
                GROUP BY s.CD_ENDERECO
            ";

            var agrupador = criterio.Agrupador;
            var volumenProductos = agrupador.Volumen;

            if (!volumenProductos.HasValue
                && !criterio.Productos.Any(p => !p.Volumen.HasValue))
            {
                volumenProductos = criterio.Productos.Sum(p => p.CantidadSeparar * p.Volumen.Value);
            }

            if (volumenProductos.HasValue)
            {
                parametrosSql["VOLUMEN_PRODUCTOS"] = volumenProductos;
                parametrosSql["PORCENTAJE_OCUPACION"] = porcentajeOcupacion;

                if (logica.Logica == AlmacenamientoDb.LOGICA_UBICACIONES_VACIAS)
                {
                    ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        INNER JOIN T_TIPO_ENDERECO te ON te.CD_TIPO_ENDERECO = ee.CD_TIPO_ENDERECO
                        WHERE te.VL_ALTURA IS NOT NULL
                            AND te.VL_COMPRIMENTO IS NOT NULL
                            AND te.VL_LARGURA IS NOT NULL
                            AND :VOLUMEN_PRODUCTOS <= (te.VL_ALTURA * te.VL_COMPRIMENTO * te.VL_LARGURA * (:PORCENTAJE_OCUPACION / 100))
                    ";
                }
                else
                {
                    ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        INNER JOIN T_TIPO_ENDERECO te ON te.CD_TIPO_ENDERECO = ee.CD_TIPO_ENDERECO
                        INNER JOIN ({volumenOcupadoSql}) vo ON vo.CD_ENDERECO = ee.CD_ENDERECO
                        WHERE te.VL_ALTURA IS NOT NULL
                            AND te.VL_COMPRIMENTO IS NOT NULL
                            AND te.VL_LARGURA IS NOT NULL
                            AND vo.QT_NULL_CUBAGEM = 0
                            AND :VOLUMEN_PRODUCTOS <= (te.VL_ALTURA * te.VL_COMPRIMENTO * te.VL_LARGURA * (:PORCENTAJE_OCUPACION / 100)) - vo.VL_CUBAGEM
                    ";
                }
            }

            return ubicacionesSql;
        }

        protected virtual string GetFiltrarUbicacionesPorDimensionesSql(CriterioAlmacenaje criterio, string ubicacionesSql, Dictionary<string, object> parametrosSql)
        {
            var agrupador = criterio.Agrupador;

            if (agrupador.Alto.HasValue
                || agrupador.Ancho.HasValue
                || agrupador.Profundidad.HasValue)
            {
                ubicacionesSql = GetFiltrarUbicacionesPorDimensionesSql(ubicacionesSql, parametrosSql, agrupador.Alto, agrupador.Ancho, agrupador.Profundidad);
            }
            else
            {
                foreach (var producto in criterio.Productos)
                {
                    ubicacionesSql = GetFiltrarUbicacionesPorDimensionesSql(ubicacionesSql, parametrosSql, producto.Alto, producto.Ancho, producto.Profundidad);
                }
            }

            return ubicacionesSql;
        }

        protected virtual string GetFiltrarUbicacionesPorPesoSql(CriterioAlmacenaje criterio, InstanciaLogica logica, string ubicacionesSql, Dictionary<string, object> parametrosSql)
        {
            var pesoOcupadoSql = @"
                SELECT 
                    s.CD_ENDERECO,
                    SUM ((COALESCE(s.QT_ESTOQUE, 0) + COALESCE(s.QT_TRANSITO_ENTRADA, 0)) * COALESCE(p.PS_BRUTO, 0)) AS PS_BRUTO,
                    SUM (CASE WHEN p.PS_BRUTO IS NULL THEN 1 ELSE 0 END) AS QT_NULL_PESO
                FROM T_ENDERECO_ESTOQUE ee 
                LEFT JOIN T_STOCK s ON s.CD_ENDERECO = ee.CD_ENDERECO
                INNER JOIN T_PRODUTO p ON p.CD_EMPRESA = s.CD_EMPRESA
                    AND p.CD_PRODUTO = s.CD_PRODUTO
                GROUP BY s.CD_ENDERECO
            ";

            var agrupador = criterio.Agrupador;
            var pesoProductos = agrupador.Peso;

            if (!pesoProductos.HasValue
                && !criterio.Productos.Any(p => !p.Peso.HasValue))
            {
                pesoProductos = criterio.Productos.Sum(p => p.CantidadSeparar * p.Peso.Value);
            }

            if (pesoProductos.HasValue)
            {
                parametrosSql["PESO_PRODUCTOS"] = pesoProductos;

                if (logica.Logica == AlmacenamientoDb.LOGICA_UBICACIONES_VACIAS)
                {
                    ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        INNER JOIN T_TIPO_ENDERECO te ON te.CD_TIPO_ENDERECO = ee.CD_TIPO_ENDERECO
                        WHERE te.VL_PESO_MAXIMO IS NOT NULL
                            AND :PESO_PRODUCTOS <= te.VL_PESO_MAXIMO
                    ";
                }
                else
                {
                    ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        INNER JOIN T_TIPO_ENDERECO te ON te.CD_TIPO_ENDERECO = ee.CD_TIPO_ENDERECO
                        INNER JOIN ({pesoOcupadoSql}) po ON po.CD_ENDERECO = ee.CD_ENDERECO
                        WHERE te.VL_PESO_MAXIMO IS NOT NULL
                            AND po.QT_NULL_PESO = 0
                            AND :PESO_PRODUCTOS <= te.VL_PESO_MAXIMO - po.PS_BRUTO
                    ";
                }
            }

            return ubicacionesSql;
        }

        protected virtual string GetFiltrarUbicacionesPorDimensionesSql(string ubicacionesSql, Dictionary<string, object> parametrosSql, decimal? alto, decimal? ancho, decimal? profundidad)
        {
            if (alto.HasValue
                || ancho.HasValue
                || profundidad.HasValue)
            {
                parametrosSql["ALTO"] = alto;
                parametrosSql["ANCHO"] = ancho;
                parametrosSql["PROFUNDIDAD"] = profundidad;

                ubicacionesSql = @$"
                        SELECT ee.*
                        FROM ({ubicacionesSql}) ee
                        INNER JOIN T_TIPO_ENDERECO te ON te.CD_TIPO_ENDERECO = ee.CD_TIPO_ENDERECO
                        WHERE (te.VL_ALTURA IS NULL OR :ALTO IS NULL OR :ALTO <= te.VL_ALTURA)
                            AND (te.VL_LARGURA IS NULL OR :ANCHO IS NULL OR :ANCHO <= te.VL_LARGURA)
                            AND (te.VL_COMPRIMENTO IS NULL OR :PROFUNDIDAD IS NULL OR :PROFUNDIDAD <= te.VL_COMPRIMENTO)
                    ";
            }

            return ubicacionesSql;
        }

        protected virtual T_ALM_SUGERENCIA GetSugerenciaAlmacenamiento(SugerenciaAlmacenamiento sugerencia)
        {
            return _context.T_ALM_SUGERENCIA.AsNoTracking()
                .Include("T_ALM_SUGERENCIA_DET")
                .FirstOrDefault(s => s.CD_AGRUPADOR == sugerencia.Agrupador
                    && s.CD_ALM_OPERATIVA_ASOCIABLE == sugerencia.Operativa.Codigo
                    && s.CD_CLASSE == sugerencia.Clase
                    && s.CD_EMPRESA == sugerencia.Empresa
                    && s.CD_ENDERECO_SUGERIDO == sugerencia.UbicacionSugerida
                    && s.CD_GRUPO == sugerencia.Grupo
                    && s.CD_PRODUTO == sugerencia.Producto
                    && s.CD_REFERENCIA == sugerencia.Referencia
                    && s.NU_ALM_ESTRATEGIA == sugerencia.Estrategia
                    && s.NU_PREDIO == sugerencia.Predio
                    && s.TP_ALM_OPERATIVA_ASOCIABLE == sugerencia.Operativa.Tipo
                    && s.NU_ALM_SUGERENCIA == sugerencia.NuAlmSugerencia);
        }

        protected virtual T_ALM_SUGERENCIA GetSugerenciaAlmacenamientoSinDetalles(SugerenciaAlmacenamiento sugerencia)
        {
            return _context.T_ALM_SUGERENCIA.AsNoTracking()
                .FirstOrDefault(s => s.CD_AGRUPADOR == sugerencia.Agrupador
                    && s.CD_ALM_OPERATIVA_ASOCIABLE == sugerencia.Operativa.Codigo
                    && s.CD_CLASSE == sugerencia.Clase
                    && s.CD_EMPRESA == sugerencia.Empresa
                    && s.CD_ENDERECO_SUGERIDO == sugerencia.UbicacionSugerida
                    && s.CD_GRUPO == sugerencia.Grupo
                    && s.CD_PRODUTO == sugerencia.Producto
                    && s.CD_REFERENCIA == sugerencia.Referencia
                    && s.NU_ALM_ESTRATEGIA == sugerencia.Estrategia
                    && s.NU_PREDIO == sugerencia.Predio
                    && s.TP_ALM_OPERATIVA_ASOCIABLE == sugerencia.Operativa.Tipo
                    && s.NU_ALM_SUGERENCIA == sugerencia.NuAlmSugerencia);
        }

        protected virtual List<T_ALM_SUGERENCIA_DET> GetDetallesSugerenciaAlmacenamiento(SugerenciaAlmacenamiento sugerencia, List<ProductoAlmacenaje> productos)
        {
            var detalles = _context.T_ALM_SUGERENCIA_DET.AsNoTracking()
                 .Where(s => s.CD_AGRUPADOR == sugerencia.Agrupador
                     && s.CD_ALM_OPERATIVA_ASOCIABLE == sugerencia.Operativa.Codigo
                     && s.CD_CLASSE == sugerencia.Clase
                     && s.CD_EMPRESA == sugerencia.Empresa
                     && s.CD_ENDERECO_SUGERIDO == sugerencia.UbicacionSugerida
                     && s.CD_GRUPO == sugerencia.Grupo
                     && s.CD_PRODUTO == sugerencia.Producto
                     && s.CD_REFERENCIA == sugerencia.Referencia
                     && s.NU_ALM_ESTRATEGIA == sugerencia.Estrategia
                     && s.NU_PREDIO == sugerencia.Predio
                     && s.TP_ALM_OPERATIVA_ASOCIABLE == sugerencia.Operativa.Tipo
                     && s.NU_ALM_SUGERENCIA == sugerencia.NuAlmSugerencia).ToList();

            return detalles.Join(productos,
                de => new { de.CD_PRODUTO_AGRUPADOR, de.NU_IDENTIFICADOR_AGRUPADOR, de.CD_FAIXA_AGRUPADOR, de.CD_EMPRESA_AGRUPADOR, de.CD_ENDERECO_SUGERIDO },
                pr => new { CD_PRODUTO_AGRUPADOR = pr.Codigo, NU_IDENTIFICADOR_AGRUPADOR = pr.Lote, CD_FAIXA_AGRUPADOR = pr.Faixa, CD_EMPRESA_AGRUPADOR = pr.Empresa, CD_ENDERECO_SUGERIDO = pr.UbicacionDestino },
                (de, pr) => de).ToList();
        }

        protected virtual T_ALM_SUGERENCIA AddSugerencia(T_ALM_SUGERENCIA sugerencia)
        {
            return AddSugerencia(sugerencia);
        }

        protected virtual T_ALM_SUGERENCIA AddSugerencia(T_ALM_SUGERENCIA sugerencia, List<ProductoAlmacenaje> productos, bool isConDetalles = false)
        {
            sugerencia.T_ALM_ESTRATEGIA = _context.T_ALM_ESTRATEGIA.FirstOrDefault(e => e.NU_ALM_ESTRATEGIA == sugerencia.NU_ALM_ESTRATEGIA);
            sugerencia.T_ALM_LOGICA_INSTANCIA = _context.T_ALM_LOGICA_INSTANCIA.FirstOrDefault(i => i.NU_ALM_LOGICA_INSTANCIA == sugerencia.NU_ALM_LOGICA_INSTANCIA);

            if (isConDetalles)
            {
                sugerencia.T_ALM_SUGERENCIA_DET.AddRange(productos.Select(p => new T_ALM_SUGERENCIA_DET
                {
                    NU_ALM_SUGERENCIA_DET = _context.GetNextSequenceValueLong(_dapper, "S_NU_ALM_SUGERENCIA_DET"),
                    NU_ALM_SUGERENCIA = sugerencia.NU_ALM_SUGERENCIA,
                    CD_AGRUPADOR = sugerencia.CD_AGRUPADOR,
                    CD_ALM_OPERATIVA_ASOCIABLE = sugerencia.CD_ALM_OPERATIVA_ASOCIABLE,
                    CD_CLASSE = sugerencia.CD_CLASSE,
                    CD_EMPRESA = sugerencia.CD_EMPRESA,
                    CD_EMPRESA_AGRUPADOR = p.Empresa,
                    CD_ENDERECO_SUGERIDO = sugerencia.CD_ENDERECO_SUGERIDO,
                    CD_FAIXA_AGRUPADOR = p.Faixa,
                    CD_GRUPO = sugerencia.CD_GRUPO,
                    CD_PRODUTO = sugerencia.CD_PRODUTO,
                    CD_PRODUTO_AGRUPADOR = p.Codigo,
                    CD_REFERENCIA = sugerencia.CD_REFERENCIA,
                    DT_FABRICACAO_AGRUPADOR = p.Vencimiento,
                    NU_ALM_ESTRATEGIA = sugerencia.NU_ALM_ESTRATEGIA,
                    NU_IDENTIFICADOR_AGRUPADOR = p.Lote,
                    NU_PREDIO = sugerencia.NU_PREDIO,
                    QT_PRODUTO_AGRUPADOR = p.CantidadSeparar,
                    QT_AUDITADA_AGRUPADOR = p.CantidadAuditada,
                    QT_CLASIFICADA_AGRUPADOR = p.CantidadClasificada,
                    TP_ALM_OPERATIVA_ASOCIABLE = sugerencia.TP_ALM_OPERATIVA_ASOCIABLE,
                    CD_ENDERECO_SUGERIDO_AGRUPADOR = sugerencia.CD_ENDERECO_SUGERIDO,
                    CD_ESTADO = sugerencia.CD_ESTADO,
                    NU_TRANSACCION = sugerencia.NU_TRANSACCION,
                    DT_ADDROW = DateTime.Now,
                    NU_ALM_LOGICA_INSTANCIA = null
                }));
            }

            _context.T_ALM_SUGERENCIA.Add(sugerencia);

            return sugerencia;
        }

        protected virtual void UpdateStockTransitoEntrada(List<ProductoAlmacenaje> productos, string cdEnderecoSugerido, bool restarStock, long nuTransaccionDB)
        {
            foreach (var producto in productos)
            {
                var stock = _context.T_STOCK
                    .FirstOrDefault(s => s.CD_ENDERECO == cdEnderecoSugerido
                        && s.CD_PRODUTO == producto.Codigo
                        && s.CD_FAIXA == producto.Faixa
                        && s.NU_IDENTIFICADOR == producto.Lote
                        && s.CD_EMPRESA == producto.Empresa);

                if (stock == null)
                {
                    stock = new T_STOCK
                    {
                        CD_ENDERECO = cdEnderecoSugerido,
                        CD_PRODUTO = producto.Codigo,
                        CD_FAIXA = producto.Faixa,
                        NU_IDENTIFICADOR = producto.Lote,
                        CD_EMPRESA = producto.Empresa,
                        QT_ESTOQUE = 0,
                        QT_RESERVA_SAIDA = 0,
                        DT_FABRICACAO = producto.Vencimiento,
                        QT_TRANSITO_ENTRADA = restarStock ? producto.CantidadSeparar : 0,
                        NU_TRANSACCION = nuTransaccionDB,
                    };

                    _context.T_STOCK.Add(stock);
                }

                var vencimientoStock = stock.DT_FABRICACAO ?? DateTime.Now;
                var vencimientoPallet = producto.Vencimiento ?? DateTime.Now;

                if (vencimientoStock > vencimientoPallet)
                    stock.DT_FABRICACAO = producto.Vencimiento ?? stock.DT_FABRICACAO;
                else
                    stock.DT_FABRICACAO = stock.DT_FABRICACAO ?? producto.Vencimiento;


                stock.NU_TRANSACCION = nuTransaccionDB;
                stock.QT_TRANSITO_ENTRADA = (stock.QT_TRANSITO_ENTRADA ?? 0) + (restarStock ? -1 : 1) * producto.CantidadSeparar;
            }
        }
       
        public virtual void AprobarSugerenciaParaReabastecimiento(SugerenciaAlmacenamiento sugerencia, int empresa, string producto, decimal faixa, string lote, decimal cantidad, long nuTransaccionDB, DateTime? vencimiento)
        {
            var productos = new List<ProductoAlmacenaje>
            {
                new ProductoAlmacenaje
                {
                    Empresa = empresa,
                    Codigo = producto,
                    Faixa = faixa,
                    Lote = lote,
                    CantidadSeparar = cantidad,
                    Vencimiento = vencimiento,
                }
            };

            AprobarSugerenciaReabastecimiento(sugerencia, productos, false, nuTransaccionDB);
        }

        protected virtual T_ALM_REABASTECIMIENTO GetSugerenciaReabastecimiento(SugerenciaAlmacenamiento sugerencia)
        {
            return _context.T_ALM_REABASTECIMIENTO
                .FirstOrDefault(s => s.CD_EMPRESA == sugerencia.Empresa
                    && s.CD_ENDERECO_SUGERIDO == sugerencia.UbicacionSugerida
                    && s.CD_PRODUTO == sugerencia.Producto
                    && s.CD_REFERENCIA == sugerencia.Referencia
                    && s.NU_PREDIO == sugerencia.Predio && s.CD_ESTADO == AlmacenamientoDb.ESTADO_PENDIENTE);
        }
        
        public virtual void AprobarSugerenciaReabastecimiento(SugerenciaAlmacenamiento sugerencia, List<ProductoAlmacenaje> productos, bool actualizaTransitoEntrada, long nuTransaccionDB)
        {
            var entity = GetSugerenciaReabastecimiento(sugerencia);

            entity.CD_ESTADO = AlmacenamientoDb.ESTADO_APROBADO;
            entity.DT_UPDROW = DateTime.Now;
            entity.CD_FUNCIONARIO = _userId;
            entity.NU_TRANSACCION = nuTransaccionDB;

            if (actualizaTransitoEntrada)
            {
                UpdateStockTransitoEntrada(productos, sugerencia.UbicacionSugerida, true, nuTransaccionDB);
            }

            _context.T_ALM_REABASTECIMIENTO.Attach(entity);
            _context.Entry<T_ALM_REABASTECIMIENTO>(entity).State = EntityState.Modified;
        }

        public virtual void CancelarSugerenciaParaReabastecimiento(SugerenciaAlmacenamiento sugerencia, int empresa, string producto, decimal faixa, string lote, decimal cantidad, long nuTransaccionDB)
        {
            var productos = new List<ProductoAlmacenaje>
            {
                new ProductoAlmacenaje
                {
                    Empresa = empresa,
                    Codigo = producto,
                    Faixa = faixa,
                    Lote = lote,
                    CantidadSeparar = cantidad,
                }
            };

            CancelarSugerenciaReabastecimiento(sugerencia, productos, nuTransaccionDB);
        }

        public virtual void CancelarSugerenciaReabastecimiento(SugerenciaAlmacenamiento sugerencia, List<ProductoAlmacenaje> productos, long nuTransaccionDB)
        {
            var entity = GetSugerenciaReabastecimiento(sugerencia);

            UpdateStockTransitoEntrada(productos, sugerencia.UbicacionSugerida, true, nuTransaccionDB);

            _context.T_ALM_REABASTECIMIENTO.Remove(entity);
        }

        public virtual SugerenciaAlmacenamiento SugerirUbicacionReabastecimiento(DateTime inicioCalculo, long nuTransaccionDB, string predio, int nuEtiqueta, string producto, int cdEmpresa, decimal? faixa, string lote, bool ignorarStock, decimal cantidadSeparar, decimal cantidadClasificada, decimal cantidadOriginal, DateTime? vencimiento)
        {
            var sugerencia = SugerirUbicacionReabastecimiento(inicioCalculo, predio, nuEtiqueta, cdEmpresa, producto, faixa, lote, cantidadSeparar, cantidadClasificada, cantidadOriginal, nuTransaccionDB, ignorarStock, out decimal cantidadReabastecimieto);

            if (sugerencia != null)
            {
                sugerencia.DT_FABRICACAO = vencimiento;
                UpdateStockTransitoEntrada(sugerencia.CD_ENDERECO_SUGERIDO, cdEmpresa, producto, faixa, lote, false, vencimiento, cantidadReabastecimieto, nuTransaccionDB);
                sugerencia = AddSugerencia(sugerencia);
            }

            return _mapper.Map(sugerencia);
        }

        public virtual void UpdateStockTransitoEntrada(string cdEnderecoSugerido, int cdEmpresa, string producto, decimal? faixa, string lote, bool restarStock, DateTime? vencimiento, decimal cantidad, long nuTransaccionDB)
        {
            var stock = _context.T_STOCK
                  .FirstOrDefault(s => s.CD_ENDERECO == cdEnderecoSugerido
                      && s.CD_PRODUTO == producto
                      && s.CD_FAIXA == faixa
                      && s.NU_IDENTIFICADOR == lote
                      && s.CD_EMPRESA == cdEmpresa);

            if (stock == null)
            {
                stock = new T_STOCK
                {
                    CD_ENDERECO = cdEnderecoSugerido,
                    CD_PRODUTO = producto,
                    CD_FAIXA = faixa ?? 1,
                    NU_IDENTIFICADOR = lote,
                    CD_EMPRESA = cdEmpresa,
                    QT_ESTOQUE = 0,
                    QT_RESERVA_SAIDA = 0,
                    DT_FABRICACAO = vencimiento,
                    QT_TRANSITO_ENTRADA = restarStock ? cantidad : 0,
                    NU_TRANSACCION = nuTransaccionDB,
                };

                _context.T_STOCK.Add(stock);
            }

            var vencimientoStock = stock.DT_FABRICACAO ?? DateTime.Now;
            var vencimientoPallet = vencimiento ?? DateTime.Now;

            if (vencimientoStock > vencimientoPallet)
                stock.DT_FABRICACAO = vencimiento ?? stock.DT_FABRICACAO;
            else
                stock.DT_FABRICACAO = stock.DT_FABRICACAO ?? vencimiento;


            stock.NU_TRANSACCION = nuTransaccionDB;
            stock.QT_TRANSITO_ENTRADA = (stock.QT_TRANSITO_ENTRADA ?? 0) + (restarStock ? -1 : 1) * cantidad;
        }

        public virtual decimal GetMaximoReabastecible(string predio, int cdEmpresa, string producto, decimal? faixa)
        {
            var reabastecimientos = _context.V_REC410_SUGERENCIA_REABAST.Where(x => x.NU_PREDIO == predio && x.CD_EMPRESA == cdEmpresa && x.CD_PRODUTO == producto && x.CD_FAIXA == faixa).OrderBy(x => x.QT_PADRAO_PI).ThenBy(x => x.CD_ENDERECO_PI).ToList();
            var reabastecimiento = reabastecimientos.FirstOrDefault();
            return reabastecimiento?.QT_MAXIMO_REA ?? 0;
        }

        protected virtual T_ALM_REABASTECIMIENTO SugerirUbicacionReabastecimiento(DateTime inicioCalculo, string predio, int nuEtiquetaLote, int cdEmpresa, string producto, decimal? faixa, string lote, decimal cantidadSeparar, decimal cantidadClasificada, decimal cantidadOriginal, long nuTransaccionDB, bool ignorarStock, out decimal cantidadReabastecimiento)
        {
            T_ALM_REABASTECIMIENTO sugerencia = null;
            cantidadReabastecimiento = 0;

            var reabastecimientos = _context.V_REC410_SUGERENCIA_REABAST.Where(x => x.NU_PREDIO == predio && x.CD_EMPRESA == cdEmpresa && x.CD_PRODUTO == producto && x.CD_FAIXA == faixa).OrderBy(x => x.QT_PADRAO_PI).ThenBy(x => x.CD_ENDERECO_PI).ToList();

            foreach (var reabastecimiento in reabastecimientos)
            {
                if (!ignorarStock)
                {
                    if (_context.V_STOCK_PARA_REABASTECIMIENTO.Any(x => x.CD_ENDERECO_PI == reabastecimiento.CD_ENDERECO_PI && x.CD_PRODUTO == producto && x.CD_EMPRESA == cdEmpresa && x.CD_FAIXA == faixa && x.ID_MENOS_MINIMO_PI == "S" && x.ID_SIRVE_PARA_PI == "S"))
                    {
                        continue;
                    }
                }

                var etiqueta = _mapperEtiqueta.MapToObject(_context.T_ETIQUETA_LOTE.AsNoTracking().FirstOrDefault(x => x.NU_ETIQUETA_LOTE == nuEtiquetaLote));

                sugerencia = new T_ALM_REABASTECIMIENTO();
                sugerencia.NU_ETIQUETA_LOTE = nuEtiquetaLote;
                sugerencia.CD_REFERENCIA = etiqueta.CodigoBarras;
                sugerencia.NU_PREDIO = predio;
                sugerencia.CD_ENDERECO_SUGERIDO = reabastecimiento.CD_ENDERECO_PI;
                sugerencia.NU_IDENTIFICADOR = lote;
                sugerencia.CD_PRODUTO = producto;
                sugerencia.CD_FAIXA = faixa ?? 1;
                sugerencia.CD_EMPRESA = cdEmpresa;
                sugerencia.DT_ADDROW = inicioCalculo;
                sugerencia.CD_ESTADO = AlmacenamientoDb.ESTADO_PENDIENTE;
                sugerencia.CD_FUNCIONARIO = _userId;
                sugerencia.NU_TRANSACCION = nuTransaccionDB;
                sugerencia.FL_IGNORAR_STOCK = ignorarStock ? "S" : "N";

                decimal padron = reabastecimiento.QT_PADRAO_PI;

                cantidadReabastecimiento = reabastecimiento.QT_MAXIMO_REA ?? 0;

                if (cantidadSeparar < cantidadReabastecimiento)
                {
                    cantidadReabastecimiento = Math.Truncate(cantidadSeparar / reabastecimiento.QT_PADRAO_PI);
                }

                sugerencia.QT_PRODUTO = cantidadReabastecimiento;
                sugerencia.QT_AUDITADA = cantidadOriginal;
                sugerencia.QT_CLASIFICADA = cantidadClasificada;

                if (!ignorarStock)
                {
                    if (_context.V_STOCK_PARA_REABASTECIMIENTO.Any(x => x.CD_ENDERECO_PI == reabastecimiento.CD_ENDERECO_PI && x.CD_PRODUTO == producto && x.CD_EMPRESA == cdEmpresa && x.CD_FAIXA == faixa && x.ID_MENOS_MINIMO_PI == "S" && x.ID_SIRVE_PARA_PI == "S"))
                    {
                        continue;
                    }
                }

                return sugerencia;
            }

            return null;
        }

        public virtual T_ALM_REABASTECIMIENTO AddSugerencia(T_ALM_REABASTECIMIENTO sugerencia)
        {
            sugerencia.NU_ALM_REABASTECIMIENTO = this._context.GetNextSequenceValueLong(_dapper, "S_ALM_REABASTECIMIENTO");
            _context.T_ALM_REABASTECIMIENTO.Add(sugerencia);
            return sugerencia;
        }

        protected virtual void DesvincularSugerenciaParaEtiqueta(int nuEtiqueta, long nuTransaccionDB)
        {
            var etiqueta = _context.T_ETIQUETA_LOTE
                .FirstOrDefault(e => e.NU_ETIQUETA_LOTE == nuEtiqueta);

            etiqueta.CD_ENDERECO_SUGERIDO = null;
            etiqueta.NU_TRANSACCION = nuTransaccionDB;
        }

        protected virtual string GetParametro(ICollection<InstanciaLogicaParametro> parametros, string nmParametro)
        {
            return parametros.FirstOrDefault(p => p.Parametro.Nombre == nmParametro)?.Valor;
        }

        protected virtual int? GetParametroInt(ICollection<InstanciaLogicaParametro> parametros, string nmParametro)
        {
            return ToNullableInt(GetParametro(parametros, nmParametro));
        }

        protected virtual short? GetParametroShort(ICollection<InstanciaLogicaParametro> parametros, string nmParametro)
        {
            return ToNullableShort(GetParametro(parametros, nmParametro));
        }

        protected virtual decimal? GetParametroDecimal(ICollection<InstanciaLogicaParametro> parametros, string nmParametro)
        {
            return ToNullableDecimal(GetParametro(parametros, nmParametro));
        }

        protected virtual bool? GetParametroBool(ICollection<InstanciaLogicaParametro> parametros, string nmParametro)
        {
            return ToNullableBool(GetParametro(parametros, nmParametro));
        }

        protected virtual int? ToNullableInt(string s)
        {
            if (int.TryParse(s, out int n))
                return n;

            return null;
        }

        protected virtual short? ToNullableShort(string s)
        {
            if (short.TryParse(s, out short n))
                return n;

            return null;
        }

        protected virtual decimal? ToNullableDecimal(string s)
        {
            if (decimal.TryParse(s, out decimal n))
                return n;

            return null;
        }

        protected virtual bool? ToNullableBool(string s)
        {
            if (!string.IsNullOrEmpty(s))
                return s.ToUpper() == "S";

            return null;
        }

        public virtual SugerenciaAlmacenamiento GetSugerenciaAlmacenajePendiente(string codigoBarras)
        {
            return _mapper.Map(_context.T_ALM_SUGERENCIA
                .Include("T_ALM_SUGERENCIA_DET")
                .AsNoTracking()
                .FirstOrDefault(s => s.CD_REFERENCIA.ToUpper() == codigoBarras.ToUpper()
                    && s.CD_ESTADO == AlmacenamientoDb.ESTADO_PENDIENTE));
        }

        public virtual SugerenciaAlmacenamiento GetSugerenciaReabastecimientoPendiente(string codigoBarras)
        {
            return _mapper.Map(_context.T_ALM_REABASTECIMIENTO
                .AsNoTracking()
                .FirstOrDefault(s => s.CD_REFERENCIA.ToUpper() == codigoBarras.ToUpper()
                    && s.CD_ESTADO == AlmacenamientoDb.ESTADO_PENDIENTE));
        }

        public virtual void DeleteCriterioAlmacenajeTemp(int nuEtiqueta, int userId)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var sql = @"DELETE FROM T_ALM_DET_PRODUTO_TEMP WHERE NU_ETIQUETA_LOTE = :nuEtiqueta AND USERID = :userId";
            _dapper.Execute(connection, sql, param: new { nuEtiqueta = nuEtiqueta, userId = userId }, transaction: tran);

        }

        public virtual bool AnySugerenciaAlmacenajePendienteFraccionado(string codigoBarras)
        {
            return _context.T_ALM_SUGERENCIA
               .Any(s => s.CD_AGRUPADOR.ToUpper() == codigoBarras.ToUpper()
                   && s.TP_ALM_OPERATIVA_ASOCIABLE == AlmacenamientoDb.TIPO_OPERATIVA_FRACCIONADO
                   && s.CD_ESTADO == AlmacenamientoDb.ESTADO_PENDIENTE);
        }

        public virtual bool AnySugerenciaAlmacenajePendienteClasificacion(string codigoBarras)
        {
            return _context.T_ALM_SUGERENCIA
                .Any(s => s.CD_REFERENCIA.ToUpper() == codigoBarras.ToUpper()
                    && s.TP_ALM_OPERATIVA_ASOCIABLE == AlmacenamientoDb.TIPO_OPERATIVA_CLASIFICACION
                    && s.CD_ESTADO == AlmacenamientoDb.ESTADO_PENDIENTE);
        }

        public virtual bool AnySugerenciaAlmacenajeReabastecimintoPendienteClasificacion(int nuEtiquetaLote)
        {
            return _context.T_ALM_REABASTECIMIENTO
                .Any(s => s.NU_ETIQUETA_LOTE == nuEtiquetaLote
                    && s.CD_ESTADO == AlmacenamientoDb.ESTADO_PENDIENTE);
        }

    }
}
