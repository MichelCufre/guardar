using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Utils;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class StockRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly StockMapper _mapper;
        protected readonly IDapper _dapper;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public StockRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new StockMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyStockUbicacion(string idUbicacion)
        {
            return _context.T_STOCK
                .AsNoTracking()
                .Any(s => s.CD_ENDERECO == idUbicacion);
        }

        public virtual bool AnyStockPositivoUbicacion(string idUbicacion)
        {
            return _context.T_STOCK
                .AsNoTracking()
                .Any(x => x.QT_ESTOQUE > 0 && x.CD_ENDERECO == idUbicacion);
        }

        public virtual bool UbicacionConReserva(string codigoUbicacion)
        {
            return _context.T_STOCK
                .AsNoTracking()
                .Any(x => x.QT_RESERVA_SAIDA > 0 && x.CD_ENDERECO == codigoUbicacion);
        }

        public virtual bool AnyStockUbicacion(string produto, int empresa)
        {
            return _context.T_STOCK
                .AsNoTracking()
                .Any(s => s.CD_EMPRESA == empresa
                    && s.CD_PRODUTO == produto);
        }

        public virtual bool AnyStockDisponibleUbicacion(string produto, int empresa)
        {
            return _context.T_STOCK
                .AsNoTracking()
                .Any(s => s.CD_EMPRESA == empresa
                    && s.CD_PRODUTO == produto
                    && (s.QT_ESTOQUE - s.QT_RESERVA_SAIDA) > 0);
        }

        public virtual bool AnyStockOtroProducto(int empresa, string producto, string idUbicacion)
        {
            return _context.T_STOCK
                .AsNoTracking()
                .Any(s => s.CD_ENDERECO == idUbicacion
                    && (s.CD_EMPRESA != empresa || s.CD_PRODUTO != producto));
        }

        public bool AnyStockConSerieDuplicada(string codigo, int empresa, decimal faixa)
        {
            return _context.T_STOCK.AsNoTracking()
                .Where(s => s.CD_PRODUTO == codigo &&
                            s.CD_EMPRESA == empresa &&
                            s.CD_FAIXA == faixa)
                .GroupBy(s => s.NU_IDENTIFICADOR)
                .Any(g => g.Sum(x => x.QT_ESTOQUE) > 1);
        }

        public virtual bool MasDeUnProductoEnUbicacion(string idUbicacion)
        {
            return _context.T_STOCK
                .AsNoTracking()
                .Where(w => w.CD_ENDERECO == idUbicacion)
                .Select(w => new { w.CD_EMPRESA, w.CD_PRODUTO })
                .GroupBy(w => new { w.CD_EMPRESA, w.CD_PRODUTO })
                .Count() > 1;
        }

        public virtual bool MasDeUnLoteEnUbicacion(string idUbicacion)
        {
            return _context.T_STOCK
                .AsNoTracking()
                .Where(w => w.CD_ENDERECO == idUbicacion)
                .Select(w => new { w.CD_EMPRESA, w.CD_PRODUTO, w.NU_IDENTIFICADOR })
                .GroupBy(w => new { w.CD_EMPRESA, w.CD_PRODUTO })
                .Any(w => w.Min(m => m.NU_IDENTIFICADOR) != w.Max(m => m.NU_IDENTIFICADOR));
        }

        public virtual bool ExisteSerie(string codigoProducto, int cdEmpresa, string nuIdentificador)
        {
            return _context.T_STOCK
                .Any(s => s.CD_EMPRESA == cdEmpresa
                    && s.CD_PRODUTO == codigoProducto
                    && s.NU_IDENTIFICADOR == nuIdentificador
                    && (s.QT_ESTOQUE + (s.QT_TRANSITO_ENTRADA ?? 0)) > 0);
        }

        public virtual bool ExisteSerieEnOtraUbicacion(string ubicacion, string codigoProducto, int cdEmpresa, string nuIdentificador)
        {
            return _context.T_STOCK
                .Any(s => s.CD_ENDERECO != ubicacion
                    && s.CD_EMPRESA == cdEmpresa
                    && s.CD_PRODUTO == codigoProducto
                    && s.NU_IDENTIFICADOR == nuIdentificador
                    && (s.QT_ESTOQUE + (s.QT_TRANSITO_ENTRADA ?? 0)) > 0);
        }

        #endregion

        #region Get

        public virtual List<StockAVencer> GetStockProximoAVencer(int empresa, DateTime fechaLimite, string predio)
        {
            var stock = new List<StockAVencer>();

            fechaLimite = fechaLimite.Date.AddDays(1);

            stock.AddRange(_context.V_STOCK_SUELTO
                .Join(_context.T_PRODUTO,
                    s => new { s.CD_EMPRESA, s.CD_PRODUTO },
                    p => new { p.CD_EMPRESA, p.CD_PRODUTO },
                    (s, p) => new { Stock = s, Producto = p })
                .Join(_context.T_ENDERECO_ESTOQUE,
                    sp => new { sp.Stock.CD_ENDERECO },
                    ee => new { ee.CD_ENDERECO },
                    (sp, ee) => new { sp.Stock, sp.Producto, Ubicacion = ee })
                .Where(x => x.Stock.CD_EMPRESA == empresa
                    && x.Producto.TP_MANEJO_FECHA == ManejoFechaProductoDb.Expirable
                    && x.Stock.DT_FABRICACAO != null
                    && x.Stock.DT_FABRICACAO >= DateTime.Today
                    && x.Stock.DT_FABRICACAO < fechaLimite
                    && (string.IsNullOrEmpty(predio) || x.Ubicacion.NU_PREDIO.ToUpper() == predio.ToUpper()))
                .GroupBy(x => new
                {
                    x.Ubicacion.NU_PREDIO,
                    x.Stock.CD_ENDERECO,
                    x.Stock.CD_PRODUTO,
                    x.Stock.CD_FAIXA,
                    x.Stock.NU_IDENTIFICADOR,
                    x.Stock.DT_FABRICACAO,
                    x.Stock.ID_AVERIA,
                })
                .Select(x => new StockAVencer
                {
                    Averiado = x.Key.ID_AVERIA,
                    Lote = x.Key.NU_IDENTIFICADOR,
                    Predio = x.Key.NU_PREDIO,
                    Producto = x.Key.CD_PRODUTO,
                    Ubicacion = x.Key.CD_ENDERECO,
                    Vencimiento = x.Key.DT_FABRICACAO.Value.ToString("yyyy-MM-dd"),
                })
                .ToList());

            stock.AddRange(_context.V_STOCK_LPN
                .Join(_context.T_PRODUTO,
                    s => new { s.CD_EMPRESA, s.CD_PRODUTO },
                    p => new { p.CD_EMPRESA, p.CD_PRODUTO },
                    (s, p) => new { Stock = s, Producto = p })
                .Join(_context.T_LPN,
                    sp => new { sp.Stock.NU_LPN },
                    l => new { l.NU_LPN },
                    (sp, l) => new { sp.Stock, sp.Producto, Lpn = l })
                .Join(_context.T_ENDERECO_ESTOQUE,
                    spl => new { spl.Stock.CD_ENDERECO },
                    ee => new { ee.CD_ENDERECO },
                    (spl, ee) => new { spl.Stock, spl.Producto, spl.Lpn, Ubicacion = ee })
                .Where(x => x.Stock.CD_EMPRESA == empresa
                    && x.Producto.TP_MANEJO_FECHA == ManejoFechaProductoDb.Expirable
                    && x.Stock.DT_FABRICACAO != null
                    && x.Stock.DT_FABRICACAO >= DateTime.Today
                    && x.Stock.DT_FABRICACAO < fechaLimite
                    && (string.IsNullOrEmpty(predio) || x.Ubicacion.NU_PREDIO.ToUpper() == predio.ToUpper()))
                .GroupBy(x => new
                {
                    x.Ubicacion.NU_PREDIO,
                    x.Stock.CD_ENDERECO,
                    x.Lpn.TP_LPN_TIPO,
                    x.Stock.NU_LPN,
                    x.Stock.ID_LPN_DET,
                    x.Lpn.ID_LPN_EXTERNO,
                    x.Stock.CD_PRODUTO,
                    x.Stock.CD_FAIXA,
                    x.Stock.NU_IDENTIFICADOR,
                    x.Stock.ID_AVERIA,
                })
                .Select(x => new StockAVencer
                {
                    Averiado = x.Key.ID_AVERIA,
                    Lote = x.Key.NU_IDENTIFICADOR,
                    TipoLpn = x.Key.TP_LPN_TIPO,
                    NumeroLpn = x.Key.NU_LPN.ToString(),
                    IdExternoLpn = x.Key.ID_LPN_EXTERNO,
                    Predio = x.Key.NU_PREDIO,
                    Producto = x.Key.CD_PRODUTO,
                    Ubicacion = x.Key.CD_ENDERECO,
                    Vencimiento = x.Min(y => y.Stock.DT_FABRICACAO.Value).ToString("yyyy-MM-dd"),
                })
                .ToList());

            return stock;
        }

        public virtual List<ProductoContenedorPuerta> GetStockContePuertaCamion(int cdCamion, string puertaEmbarque)
        {
            List<ProductoContenedorPuerta> list = new List<ProductoContenedorPuerta>();
            List<V_CANT_CONTE_PUERTA_WEXP> ColUbicPuerta = this._context.V_CANT_CONTE_PUERTA_WEXP.AsNoTracking().Where(x => x.CD_CAMION == cdCamion && x.CD_ENDERECO == puertaEmbarque).ToList();

            foreach (var a in ColUbicPuerta)
            {
                ProductoContenedorPuerta sto = new ProductoContenedorPuerta()
                {
                    CodigoUbicacion = a.CD_ENDERECO,
                    CodigoProducto = a.CD_PRODUTO,
                    CodigoCliente = a.CD_CLIENTE,
                    CodigoEmpresa = a.CD_EMPRESA,
                    CodigoFaixa = a.CD_FAIXA,
                    CodigoCamion = a.CD_CAMION,
                    Lote = a.NU_IDENTIFICADOR,
                    NumeroPedido = a.NU_PEDIDO,
                    EspecificaLote = this._mapper.MapStringToBoolean(a.ID_ESPECIFICA_IDENTIFICADOR),
                    CantidadPreparada = a.QT_PREPARADO
                };

                list.Add(sto);
            }

            return list;
        }

        public virtual Stock GetStock(int idEmpresa, string codigoProducto, decimal faixa, string idUbicacion, string identificador)
        {
            T_STOCK entity = this._context.T_STOCK
                .AsNoTracking()
                .Where(x => x.CD_ENDERECO == idUbicacion
                    && x.CD_EMPRESA == idEmpresa
                    && x.CD_PRODUTO == codigoProducto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador)
                .FirstOrDefault();

            if (entity == null)
                return null;
            else
                return this._mapper.MapToStock(entity);
        }

        public virtual List<Stock> GetStockProduccion(int idEmpresa, string codigoProducto, decimal faixa, string idUbicacion, string identificador)
        {
            List<Stock> entity = this._context.T_STOCK
                .AsNoTracking()
                .Where(x => x.CD_ENDERECO == idUbicacion
                    && x.CD_EMPRESA == idEmpresa
                    && x.CD_PRODUTO == codigoProducto
                    && x.CD_FAIXA == faixa
                    && (x.NU_IDENTIFICADOR == identificador || identificador == ManejoIdentificadorDb.IdentificadorAuto))
                .Select(x => this._mapper.MapToStock(x)).ToList();

            return entity;
        }

        public virtual Stock GetStock(int idEmpresa, string codigoProducto, decimal faixa, string idUbicacion)
        {
            T_STOCK entity = this._context.T_STOCK
                .AsNoTracking()
                .Where(x => x.CD_ENDERECO == idUbicacion
                    && x.CD_EMPRESA == idEmpresa
                    && x.CD_PRODUTO == codigoProducto
                    && x.CD_FAIXA == faixa)
                .FirstOrDefault();

            if (entity == null)
                return null;
            else
                return this._mapper.MapToStock(entity);
        }

        public virtual List<Stock> GetFilteredStock(string predio, Producto producto, decimal faixa, string lote = null, string idUbicacion = null)
        {
            Expression<Func<T_STOCK, bool>> expression = x =>
                x.CD_EMPRESA == producto.CodigoEmpresa &&
                x.CD_PRODUTO == producto.Codigo &&
                x.CD_FAIXA == faixa;

            if (lote != null)
                expression = expression.And(x => x.NU_IDENTIFICADOR == lote);

            if (idUbicacion != null)
                expression = expression.And(x => x.CD_ENDERECO == idUbicacion);

            List<Stock> toReturn = (from stk in this._context.T_STOCK
                                    join ee in this._context.T_ENDERECO_ESTOQUE on stk.CD_ENDERECO equals ee.CD_ENDERECO
                                    where ee.NU_PREDIO == predio
                                    select stk).AsNoTracking()
                                    .Where(expression)
                                    .AsEnumerable()
                                    .Select(_mapper.MapToStock)
                                    .ToList();

            if (toReturn.Count == 0)
                return null;
            else return toReturn;
        }

        public virtual decimal GetTotalStockGeneralByProductoEmpresaPredio(int empresa, string producto, string predio)
        {
            if (string.IsNullOrEmpty(predio) || predio == GeneralDb.PredioSinDefinir)
            {
                return (
                  from sto in _context.T_STOCK.AsNoTracking()
                  join ubi in _context.T_ENDERECO_ESTOQUE on sto.CD_ENDERECO equals ubi.CD_ENDERECO
                  join pre in _context.T_PREDIO_USUARIO on ubi.NU_PREDIO equals pre.NU_PREDIO
                  where sto.CD_EMPRESA == empresa
                    && sto.CD_PRODUTO == producto
                    && pre.USERID == _userId
                  select (sto.QT_ESTOQUE ?? 0)
                  )
                  .ToList()
                  .Sum();
            }
            else
            {
                return (
                    from sto in _context.T_STOCK.AsNoTracking()
                    join ubi in _context.T_ENDERECO_ESTOQUE on sto.CD_ENDERECO equals ubi.CD_ENDERECO
                    where sto.CD_EMPRESA == empresa
                        && sto.CD_PRODUTO == producto
                        && ubi.NU_PREDIO == predio
                    select (sto.QT_ESTOQUE ?? 0)
                    )
                    .ToList()
                    .Sum();
            }
        }

        public virtual List<string> GetUbicacionesConOSinStock(string producto, int empresa)
        {
            return this._context.T_STOCK
                .AsNoTracking()
                .Where(x => x.CD_EMPRESA == empresa && x.CD_PRODUTO == producto)?
                .Select(x => x.CD_ENDERECO)
                .ToList();
        }

        public virtual List<Stock> GetStockUbicacionConReserva(string cdUbicacion)
        {
            List<Stock> list = new List<Stock>();

            var lisstockBd = _context.T_STOCK
                .AsNoTracking()
                .Where(d => d.CD_ENDERECO == cdUbicacion && d.QT_RESERVA_SAIDA > 0)
                .ToList();

            foreach (var a in lisstockBd)
            {
                Stock stock = _mapper.MapToStock(a);
                list.Add(stock);
            }
            return list;
        }

        public virtual TipoMovimiento GetTipoMovimientoProducto(short codigoMotivimiento)
        {
            var entity = _context.T_TIPO_MOVIMIENTO.FirstOrDefault(t => t.CD_MOVIMIENTO == codigoMotivimiento);

            return _mapper.Map(entity);
        }

        #endregion

        #region Add

        public virtual void AddStock(Stock sto)
        {
            T_STOCK entity = this._mapper.MapFromStock(sto);
            this._context.T_STOCK.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateStock(Stock sto)
        {
            T_STOCK entity = this._mapper.MapFromStock(sto);
            T_STOCK attachedEntity = _context.T_STOCK.Local
                .FirstOrDefault(x => x.CD_ENDERECO == sto.Ubicacion
                    && x.CD_EMPRESA == sto.Empresa
                    && x.CD_PRODUTO == sto.Producto
                    && x.CD_FAIXA == sto.Faixa
                    && x.NU_IDENTIFICADOR == sto.Identificador);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_STOCK.Attach(entity);
                _context.Entry<T_STOCK>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateStocks(EntityChanges<Stock> records)
        {
            foreach (var deletedRecord in records.DeletedRecords)
            {
                this.RemoveStock(deletedRecord);
            }

            foreach (var newRecord in records.AddedRecords)
            {
                this.AddStock(newRecord);
            }

            foreach (var updatedRecord in records.UpdatedRecords)
            {
                this.UpdateStock(updatedRecord);
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveStock(Stock stockEliminar)
        {
            T_STOCK entity = this._mapper.MapFromStock(stockEliminar);
            T_STOCK attachedEntity = _context.T_STOCK.Local
                .FirstOrDefault(x => x.CD_EMPRESA == stockEliminar.Empresa
                    && x.CD_PRODUTO == stockEliminar.Producto
                    && x.CD_FAIXA == stockEliminar.Faixa
                    && x.CD_ENDERECO == stockEliminar.Ubicacion
                    && x.NU_IDENTIFICADOR == stockEliminar.Identificador);

            if (attachedEntity != null)
            {
                _context.T_STOCK.Remove(attachedEntity);
            }
            else
            {
                _context.T_STOCK.Attach(entity);
                _context.T_STOCK.Remove(entity);
            }
        }

        public virtual void RemoveListStock(List<Stock> liststockEliminar)
        {
            foreach (Stock stockEliminar in liststockEliminar)
            {
                this.RemoveStock(stockEliminar);
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<Stock> GetStocksCriterios(IEnumerable<CriterioControlCalidadAPI> criterios)
        {
            IEnumerable<Stock> resultado = new List<Stock>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_STOCK_PREDIO_TEMP (NU_PREDIO, CD_EMPRESA, CD_PRODUTO, NU_IDENTIFICADOR, CD_FAIXA) 
                                   VALUES (:Predio, :Empresa, :Producto, :Lote, :Faixa)";
                    _dapper.Execute(connection, sql, criterios, transaction: tran);

                    sql = @"SELECT
                        sto.CD_EMPRESA as Empresa,
                        sto.CD_ENDERECO as Ubicacion,
                        sto.CD_FAIXA as Faixa,
                        sto.CD_MOTIVO_AVERIA as MotivoAveria,
                        sto.CD_PRODUTO as Producto,
                        sto.DT_FABRICACAO as Vencimiento,
                        sto.DT_INVENTARIO as FechaInventario,
                        sto.DT_UPDROW as FechaModificacion,
                        sto.ID_AVERIA as Averia,
                        sto.ID_CTRL_CALIDAD as ControlCalidad,
                        sto.ID_INVENTARIO as Inventario,
                        sto.NU_IDENTIFICADOR as Identificador,
                        sto.NU_TRANSACCION as NumeroTransaccion,
                        sto.QT_ESTOQUE as Cantidad,
                        sto.QT_RESERVA_SAIDA as ReservaSalida,
                        sto.QT_TRANSITO_ENTRADA as CantidadTransitoEntrada,
                        EE.NU_PREDIO as Predio
                        FROM T_STOCK sto
                        INNER JOIN T_ENDERECO_ESTOQUE EE ON sto.CD_ENDERECO = EE.CD_ENDERECO  
                        INNER JOIN T_STOCK_PREDIO_TEMP T ON sto.CD_EMPRESA = T.CD_EMPRESA                       
                        AND sto.CD_PRODUTO = T.CD_PRODUTO
                        AND sto.CD_FAIXA = T.CD_FAIXA
                        AND sto.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR
                        AND EE.NU_PREDIO = T.NU_PREDIO";

                    resultado = _dapper.Query<Stock>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Stock> GetStocks(IEnumerable<Stock> stocks)
        {
            IEnumerable<Stock> resultado = new List<Stock>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_STOCK_TEMP (CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, NU_IDENTIFICADOR, CD_FAIXA) 
                                   VALUES (:Ubicacion, :Empresa, :Producto, :Identificador, :Faixa)";
                    _dapper.Execute(connection, sql, stocks, transaction: tran);

                    sql = GetSqlSelectStock() +
                        @"  INNER JOIN T_STOCK_TEMP T ON sto.CD_EMPRESA = T.CD_EMPRESA
                            AND sto.CD_ENDERECO = T.CD_ENDERECO                            
                            AND sto.CD_FAIXA = T.CD_FAIXA
                            AND sto.CD_PRODUTO = T.CD_PRODUTO
                            AND sto.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR";

                    resultado = _dapper.Query<Stock>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Stock> GetStock(IEnumerable<Stock> keys)
        {
            IEnumerable<Stock> resultado = new List<Stock>();

            string sql = @"INSERT INTO T_STOCK_TEMP (CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, NU_IDENTIFICADOR, CD_FAIXA, IDROW) 
                           VALUES (:Ubicacion, :Empresa, :Producto, :Identificador, :Faixa, :NumeroTransaccion)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"SELECT 
                        sto.CD_EMPRESA as Empresa,
                        sto.CD_ENDERECO as Ubicacion,
                        sto.CD_FAIXA as Faixa,
                        sto.CD_MOTIVO_AVERIA as MotivoAveria,
                        sto.CD_PRODUTO as Producto,
                        sto.DT_FABRICACAO as Vencimiento,
                        sto.DT_INVENTARIO as FechaInventario,
                        sto.DT_UPDROW as FechaModificacion,
                        sto.ID_AVERIA as Averia,
                        sto.ID_CTRL_CALIDAD as ControlCalidad,
                        sto.ID_INVENTARIO as Inventario,
                        sto.NU_IDENTIFICADOR as Identificador,
                        sto.NU_TRANSACCION as NumeroTransaccion,
                        sto.QT_ESTOQUE as Cantidad,
                        sto.QT_RESERVA_SAIDA as ReservaSalida,
                        sto.QT_TRANSITO_ENTRADA as CantidadTransitoEntrada,
                        ee.NU_PREDIO as Predio
                    FROM T_STOCK sto 
                    INNER JOIN T_STOCK_TEMP T ON sto.CD_EMPRESA = T.CD_EMPRESA
                    AND sto.CD_ENDERECO = T.CD_ENDERECO                            
                    AND sto.CD_FAIXA = T.CD_FAIXA
                    AND sto.CD_PRODUTO = T.CD_PRODUTO
                    AND (sto.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR OR T.NU_IDENTIFICADOR = '(AUTO)')
                    INNER JOIN T_ENDERECO_ESTOQUE EE ON sto.CD_ENDERECO = EE.CD_ENDERECO ";

            resultado = _dapper.Query<Stock>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"DELETE T_STOCK_TEMP WHERE IDROW = :IdRow";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param: new { IdRow = keys.FirstOrDefault().NumeroTransaccion }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return resultado;
        }

        public virtual List<StockResponse> GetStock(FiltrosStock filtros, int pageSize, DbConnection connection)
        {
            string sqlwhere = " WHERE CD_EMPRESA = :Empresa ";

            if (!string.IsNullOrEmpty(filtros.Producto))
                sqlwhere += " AND CD_PRODUTO = :Producto ";

            if (!string.IsNullOrEmpty(filtros.Ubicacion))
                sqlwhere += " AND CD_ENDERECO = :Ubicacion ";

            if (!string.IsNullOrEmpty(filtros.Clase))
                sqlwhere += " AND CD_CLASSE = :Clase ";

            if (filtros.Familia != null)
                sqlwhere += " AND CD_FAMILIA_PRODUTO = :Familia ";

            if (filtros.Ramo != null)
                sqlwhere += " AND CD_RAMO_PRODUTO = :Ramo ";

            if (!string.IsNullOrEmpty(filtros.TipoManejoFecha))
                sqlwhere += " AND TP_MANEJO_FECHA = :TipoManejoFecha ";

            if (!string.IsNullOrEmpty(filtros.ManejoIdentificador))
                sqlwhere += " AND ID_MANEJO_IDENTIFICADOR = :ManejoIdentificador ";

            if (!string.IsNullOrEmpty(filtros.Predio))
                sqlwhere += " AND NU_PREDIO = :Predio ";

            if (!string.IsNullOrEmpty(filtros.GrupoConsulta))
                sqlwhere += " AND CD_GRUPO_CONSULTA = :GrupoConsulta ";

            if (filtros.Averia)
                sqlwhere += " AND (ID_AREA_AVARIA = 'S' OR ID_AVERIA='S') ";
            else
            {
                sqlwhere += @" AND (((ID_AREA_AVARIA != 'S' AND ID_AVERIA != 'S') OR (ID_AREA_AVARIA IS NULL OR ID_AVERIA IS NULL)) 
                                AND (ID_ESTOQUE_GERAL = 'S' OR ID_AREA_PICKING = 'S')
                                AND ID_DISP_ESTOQUE = 'S' AND ID_INVENTARIO != 'D' AND ID_CTRL_CALIDAD = 'C') ";
            }

            string sql = @"SELECT                         
                        CD_PRODUTO as Producto,
                        SUM(QT_GENERAL) as StockGeneral,
                        SUM(QT_DISP) as StockDisponible FROM V_CONSULTA_STOCK " + sqlwhere + @" GROUP BY CD_PRODUTO, CD_EMPRESA ";

            string orderBy = "ORDER BY Producto ASC";

            return _dapper.QueryPage<StockResponse>(connection, sql, orderBy, param: new
            {
                Empresa = filtros.Empresa,
                Producto = filtros.Producto,
                Ubicacion = filtros.Ubicacion,
                Clase = filtros.Clase,
                Familia = filtros.Familia,
                Ramo = filtros.Ramo,
                TipoManejoFecha = filtros.TipoManejoFecha,
                ManejoIdentificador = filtros.ManejoIdentificador,
                Predio = filtros.Predio,
                GrupoConsulta = filtros.GrupoConsulta,
                pageNumber = filtros.Pagina,
                pageSize = pageSize,
            }, commandType: CommandType.Text).ToList();
        }

        public virtual ConsultaStockResponse ConsultaStock(FiltrosStock filtros, int pageSize, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                connection.OpenAsync(cancelToken);
                logger.Debug($"Consulta de stock. Filtros: {JsonConvert.SerializeObject(filtros)}");

                var consulta = new ConsultaStockResponse();
                try
                {
                    consulta.Stock = GetStock(filtros, pageSize, connection);
                    logger.Debug($"Consulta de stock terminada.");
                }
                catch (Exception ex)
                {
                    logger.Error($"Error al consultar stock. Filtros: {JsonConvert.SerializeObject(filtros)} - Error: {ex}");
                    throw ex;
                }
                return consulta;
            }
        }

        public virtual List<Stock> GetStock(string ubicacion, int empresa, string producto, decimal faixa, DbConnection connection, DbTransaction tran)
        {
            var param = new
            {
                ubicacion = ubicacion,
                empresa = empresa,
                producto = producto,
                faixa = faixa
            };

            string sql = GetSqlSelectStock() + @" 
                         WHERE sto.CD_ENDERECO = :ubicacion                  
                         AND sto.CD_EMPRESA = :empresa
                         AND sto.CD_PRODUTO = :producto 
                         AND sto.CD_FAIXA = :faixa 
                         ORDER BY DT_FABRICACAO DESC";
            return _dapper.Query<Stock>(connection, sql, param, transaction: tran).ToList();
        }

        public virtual Stock GetStock(string ubicacion, int empresa, string producto, decimal faixa, string identificador, DbConnection connection = null, DbTransaction tran = null)
        {
            connection = connection ?? _context.Database.GetDbConnection();
            tran = tran ?? _context.Database.CurrentTransaction?.GetDbTransaction();

            var param = new
            {
                ubicacion = ubicacion,
                empresa = empresa,
                producto = producto,
                faixa = faixa,
                identificador = identificador
            };

            string sql = GetSqlSelectStock() + @" 
                         WHERE sto.CD_ENDERECO = :ubicacion                  
                         AND sto.CD_EMPRESA = :empresa
                         AND sto.CD_PRODUTO = :producto 
                         AND sto.CD_FAIXA = :faixa 
                         AND sto.NU_IDENTIFICADOR = :identificador ";
            return _dapper.Query<Stock>(connection, sql, param, transaction: tran).FirstOrDefault();
        }

        public static string GetSqlSelectStock()
        {
            return @"SELECT 
                        sto.CD_EMPRESA as Empresa,
                        sto.CD_ENDERECO as Ubicacion,
                        sto.CD_FAIXA as Faixa,
                        sto.CD_MOTIVO_AVERIA as MotivoAveria,
                        sto.CD_PRODUTO as Producto,
                        sto.DT_FABRICACAO as Vencimiento,
                        sto.DT_INVENTARIO as FechaInventario,
                        sto.DT_UPDROW as FechaModificacion,
                        sto.ID_AVERIA as Averia,
                        sto.ID_CTRL_CALIDAD as ControlCalidad,
                        sto.ID_INVENTARIO as Inventario,
                        sto.NU_IDENTIFICADOR as Identificador,
                        sto.NU_TRANSACCION as NumeroTransaccion,
                        sto.QT_ESTOQUE as Cantidad,
                        sto.QT_RESERVA_SAIDA as ReservaSalida,
                        sto.QT_TRANSITO_ENTRADA as CantidadTransitoEntrada
                    FROM T_STOCK sto ";
        }

        public virtual IEnumerable<Stock> GetStocksPredio(IEnumerable<Stock> stocks)
        {
            IEnumerable<Stock> resultado = new List<Stock>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_STOCK_TEMP (CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, NU_IDENTIFICADOR, CD_FAIXA) 
                                   VALUES (:Ubicacion, :Empresa, :Producto, :Identificador, :Faixa)";
                    _dapper.Execute(connection, sql, stocks, transaction: tran);

                    sql = @"SELECT 
                                sto.CD_EMPRESA as Empresa,
                                sto.CD_ENDERECO as Ubicacion,
                                sto.CD_FAIXA as Faixa,
                                sto.CD_MOTIVO_AVERIA as MotivoAveria,
                                sto.CD_PRODUTO as Producto,
                                sto.DT_FABRICACAO as Vencimiento,
                                sto.DT_INVENTARIO as FechaInventario,
                                sto.DT_UPDROW as FechaModificacion,
                                sto.ID_AVERIA as Averia,
                                sto.ID_CTRL_CALIDAD as ControlCalidad,
                                sto.ID_INVENTARIO as Inventario,
                                sto.NU_IDENTIFICADOR as Identificador,
                                sto.NU_TRANSACCION as NumeroTransaccion,
                                sto.QT_ESTOQUE as Cantidad,
                                sto.QT_RESERVA_SAIDA as ReservaSalida,
                                sto.QT_TRANSITO_ENTRADA as CantidadTransitoEntrada,
                                ee.NU_PREDIO as Predio
                            FROM T_STOCK sto 
                            INNER JOIN T_STOCK_TEMP T ON sto.CD_EMPRESA = T.CD_EMPRESA
                            AND sto.CD_ENDERECO = T.CD_ENDERECO                            
                            AND sto.CD_FAIXA = T.CD_FAIXA
                            AND sto.CD_PRODUTO = T.CD_PRODUTO
                            AND sto.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR
                            INNER JOIN T_ENDERECO_ESTOQUE EE ON sto.CD_ENDERECO = EE.CD_ENDERECO ";

                    resultado = _dapper.Query<Stock>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Stock> GetLotesExistente(IEnumerable<Producto> keys)
        {
            IEnumerable<Stock> resultado = new List<Stock>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_STOCK_TEMP (CD_EMPRESA, CD_PRODUTO) 
                                   VALUES (:CodigoEmpresa, :Codigo)";
                    _dapper.Execute(connection, sql, keys, transaction: tran);

                    sql = @"SELECT 
                                sto.CD_EMPRESA as Empresa,
                                sto.CD_PRODUTO as Producto,
                                sto.NU_IDENTIFICADOR as Identificador
                            FROM T_STOCK sto  INNER JOIN T_STOCK_TEMP T ON 
                                sto.CD_EMPRESA = T.CD_EMPRESA AND 
                                sto.CD_PRODUTO = T.CD_PRODUTO 
                            GROUP BY sto.CD_EMPRESA,
                                sto.CD_PRODUTO,
                                sto.NU_IDENTIFICADOR";

                    resultado = _dapper.Query<Stock>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        #region Api Transferencia

        public virtual async Task TransferirStock(List<TransferenciaStock> transferencias, ITransferenciaStockServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    var bulkContext = await GetBulkOperationContext(transferencias, context, connection, tran);

                    //Carga
                    await InsertPalletTransferenciaAsync(connection, tran, bulkContext.NewPalletTransferencia);
                    await BulkInsertDetallePalletTransferencia(connection, tran, bulkContext.NewDetallesPallet);

                    await BulkUpdateStockBaja(connection, tran, bulkContext.UpdateStockOrigenBaja.Values);
                    await BulkUpdateStockAlta(connection, tran, bulkContext.UpdateStockEquipoAlta.Values, updateReserva: true);
                    await BulkInsertStock(connection, tran, bulkContext.NewStockEquipo.Values);

                    //Descarga
                    await BulkUpdateStockBaja(connection, tran, bulkContext.UpdateStockEquipoBaja.Values, updateReserva: true);
                    await BulkUpdateStockAlta(connection, tran, bulkContext.UpdateStockDestinoAlta.Values, updateReserva: false, updateVencimiento: true);
                    await BulkInsertStock(connection, tran, bulkContext.NewStockDestino.Values);

                    await BulkUpdateDetallePalletTransferencia(connection, tran, bulkContext.UpdateDetallesPallet);
                    await UpdatePalletTransferencia(connection, tran, bulkContext.UpdatePalletTransferencia);

                    tran.Commit();
                }
            }
        }

        public virtual async Task<TransferenciaBulkOperationContext> GetBulkOperationContext(List<TransferenciaStock> transferencias, ITransferenciaStockServiceContext serviceContext, DbConnection connection, DbTransaction tran)
        {
            var nroSecDetalle = 0;
            var context = new TransferenciaBulkOperationContext();
            var nroEtiqueta = GetNuEtiqueta(connection, tran);
            var ubicacionEquipo = serviceContext.UbicacionEquipo;

            var nuTransaccionCarga = await CreateTransaction($"Carga de etiqueta", connection, tran);
            var nuTransaccionDescarga = await CreateTransaction($"Descarga de etiqueta", connection, tran);

            context.NewPalletTransferencia = GetNewPalletEntity(serviceContext, nroEtiqueta, nuTransaccionCarga);

            foreach (var tr in transferencias)
            {
                var stockOrigen = serviceContext.GetStock(tr.Ubicacion, tr.Producto, tr.Empresa, tr.Identificador, tr.Faixa);

                var keyStockOrigen = $"{tr.Ubicacion}.{tr.Empresa}.{tr.Producto}.{tr.Identificador}.{tr.Faixa.ToString("#.###")}";
                if (!context.UpdateStockOrigenBaja.ContainsKey(keyStockOrigen))
                    context.UpdateStockOrigenBaja[keyStockOrigen] = GetUpdateStockEntity(tr, tr.Ubicacion, nuTransaccionCarga);
                else
                    context.UpdateStockOrigenBaja[keyStockOrigen].Cantidad += tr.Cantidad;

                context.NewDetallesPallet.Add(GetNewDetallePalletEntity(serviceContext, tr, stockOrigen, nroEtiqueta, nroSecDetalle, nuTransaccionCarga));

                var keyStockEquipo = $"{ubicacionEquipo}.{tr.Empresa}.{tr.Producto}.{tr.Identificador}.{tr.Faixa.ToString("#.###")}";
                var stockEquipo = serviceContext.GetStock(ubicacionEquipo, tr.Producto, tr.Empresa, tr.Identificador, tr.Faixa);
                if (stockEquipo != null)
                {
                    if (!context.UpdateStockEquipoAlta.ContainsKey(keyStockEquipo))
                        context.UpdateStockEquipoAlta[keyStockEquipo] = GetUpdateStockEntity(tr, ubicacionEquipo, nuTransaccionCarga);
                    else
                        context.UpdateStockEquipoAlta[keyStockEquipo].Cantidad += tr.Cantidad;
                }
                else
                {
                    if (!context.NewStockEquipo.ContainsKey(keyStockEquipo))
                        context.NewStockEquipo[keyStockEquipo] = GetNewStockEntity(tr, stockOrigen, nuTransaccionCarga, ubicacionEquipo);
                    else
                    {
                        context.NewStockEquipo[keyStockEquipo].Cantidad += tr.Cantidad;
                        context.NewStockEquipo[keyStockEquipo].ReservaSalida += tr.Cantidad;
                    }
                }

                if (!context.UpdateStockEquipoBaja.ContainsKey(keyStockEquipo))
                    context.UpdateStockEquipoBaja[keyStockEquipo] = GetUpdateStockEntity(tr, ubicacionEquipo, nuTransaccionDescarga);
                else
                    context.UpdateStockEquipoBaja[keyStockEquipo].Cantidad += tr.Cantidad;


                var keyStockDestino = $"{tr.UbicacionDestino}.{tr.Empresa}.{tr.Producto}.{tr.Identificador}.{tr.Faixa.ToString("#.###")}";
                var stockDestino = serviceContext.GetStock(tr.UbicacionDestino, tr.Producto, tr.Empresa, tr.Identificador, tr.Faixa);
                if (stockDestino != null)
                {
                    var vencimiento = (stockOrigen.Vencimiento < stockDestino.Vencimiento) ? stockOrigen.Vencimiento : stockDestino.Vencimiento;

                    if (!context.UpdateStockDestinoAlta.ContainsKey(keyStockDestino))
                        context.UpdateStockDestinoAlta[keyStockDestino] = GetUpdateStockEntity(tr, tr.UbicacionDestino, nuTransaccionDescarga, vencimiento);
                    else
                        context.UpdateStockDestinoAlta[keyStockDestino].Cantidad += tr.Cantidad;
                }
                else
                {
                    if (!context.NewStockDestino.ContainsKey(keyStockDestino))
                        context.NewStockDestino[keyStockDestino] = GetNewStockEntity(tr, stockOrigen, nuTransaccionDescarga);
                    else
                        context.NewStockDestino[keyStockDestino].Cantidad += tr.Cantidad;
                }

                context.UpdateDetallesPallet.Add(GetUpdateDetallePalletEntity(nroEtiqueta, nroSecDetalle, nuTransaccionDescarga));
                nroSecDetalle++;
            }
            context.UpdatePalletTransferencia = GetUpdatePalletEntity(nroEtiqueta, nuTransaccionDescarga);

            return context;
        }

        public virtual async Task BulkInsertStock(DbConnection connection, DbTransaction tran, IEnumerable<object> stocks)
        {
            string sql = @"INSERT INTO T_STOCK 
                            (CD_ENDERECO,
                            CD_EMPRESA,
                            CD_PRODUTO,
                            CD_FAIXA,
                            NU_IDENTIFICADOR,
                            QT_ESTOQUE,
                            QT_RESERVA_SAIDA,
                            QT_TRANSITO_ENTRADA,
                            DT_FABRICACAO,
                            ID_AVERIA,
                            ID_INVENTARIO,
                            DT_INVENTARIO,
                            ID_CTRL_CALIDAD,
                            DT_UPDROW,
                            NU_TRANSACCION) 
                        VALUES 
                            (:Ubicacion,
                            :Empresa,
                            :Producto,
                            :Faixa,
                            :Identificador,
                            :Cantidad,
                            :ReservaSalida, 
                            :CantidadTransitoEntrada,
                            :Vencimiento,
                            :Averia, 
                            :Inventario,
                            :FechaInventario,
                            :ControlCalidad,
                            :FechaModificacion,
                            :NumeroTransaccion)";

            await _dapper.ExecuteAsync(connection, sql, stocks, transaction: tran);
        }

        public virtual async Task BulkUpdateStockBaja(DbConnection connection, DbTransaction tran, IEnumerable<object> stocks, bool updateReserva = false)
        {
            var sql = @"UPDATE T_STOCK SET NU_TRANSACCION = :NumeroTransaccion, DT_UPDROW = :FechaModificacion, QT_ESTOQUE = QT_ESTOQUE - :Cantidad ";

            if (updateReserva)
                sql += ", QT_RESERVA_SAIDA = QT_RESERVA_SAIDA - :Cantidad ";

            sql += @" WHERE CD_ENDERECO = :Ubicacion
                    AND CD_PRODUTO = :Producto
                    AND CD_FAIXA = :Faixa
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(connection, sql, stocks, transaction: tran);
        }

        public virtual async Task BulkUpdateStockAlta(DbConnection connection, DbTransaction tran, IEnumerable<object> stocks, bool updateReserva = false, bool updateVencimiento = false)
        {
            var sql = @"UPDATE T_STOCK SET NU_TRANSACCION = :NumeroTransaccion, DT_UPDROW = :FechaModificacion, QT_ESTOQUE = QT_ESTOQUE + :Cantidad ";

            if (updateReserva)
                sql += ", QT_RESERVA_SAIDA = QT_RESERVA_SAIDA + :Cantidad ";

            if (updateVencimiento)
                sql += ", DT_FABRICACAO = :Vencimiento ";

            sql += @" WHERE CD_ENDERECO = :Ubicacion
                    AND CD_PRODUTO = :Producto
                    AND CD_FAIXA = :Faixa
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(connection, sql, stocks, transaction: tran);
        }

        public virtual async Task InsertPalletTransferenciaAsync(DbConnection connection, DbTransaction tran, object pallet)
        {
            string sql = @"INSERT INTO T_PALLET_TRANSFERENCIA 
                            (NU_ETIQUETA,
                            NU_SEC_ETIQUETA,
                            CD_SITUACAO,
                            CD_ENDERECO_REAL,
                            DT_ADDROW,
                            CD_APLICACAO_ORIGEN,
                            NU_PREDIO,
                            TP_MODALIDAD_USO,
                            NU_TRANSACCION,
                            ID_EXTERNO_ETIQUETA,
                            TP_ETIQUETA_TRANSFERENCIA) 
                        VALUES 
                            (:nroEtiqueta,
                            :nroSecuencia,
                            :situacion,
                            :ubicacion, 
                            :dtAddrow,
                            :app,
                            :predio,
                            :modalidadUso,
                            :nuTransaccion,
                            :idExterno,
                            :tipoEtiqueta)";

            await _dapper.ExecuteAsync(connection, sql, pallet, transaction: tran);
        }

        public virtual async Task UpdatePalletTransferencia(DbConnection connection, DbTransaction tran, object pallet)
        {
            string sql = @"UPDATE T_PALLET_TRANSFERENCIA 
                            SET CD_SITUACAO = :situacion, 
                                DT_UPDROW = :fechaModificacion,
                                DT_FINALIZACION = :fechaModificacion,
                                NU_TRANSACCION=:nuTransaccion
                            WHERE NU_ETIQUETA = :nroEtiqueta 
                            AND NU_SEC_ETIQUETA = :nroSecuencia ";

            await _dapper.ExecuteAsync(connection, sql, pallet, transaction: tran);
        }

        public virtual async Task BulkInsertDetallePalletTransferencia(DbConnection connection, DbTransaction tran, List<object> detalles)
        {
            string sql = @"INSERT INTO T_DET_PALLET_TRANSFERENCIA 
                                (NU_ETIQUETA,
                                NU_SEC_ETIQUETA,
                                NU_SEC_DETALLE,
                                CD_ENDERECO_ORIGEN,
                                CD_EMPRESA,
                                CD_PRODUTO,
                                CD_FAIXA,
                                NU_IDENTIFICADOR,
                                CD_SITUACAO,
                                QT_PRODUTO,
                                DT_FABRICACAO,
                                ID_AVERIA,
                                ID_INVENTARIO_CICLICO,
                                DT_ULT_INVENTARIO,
                                ID_CTRL_CALI_PEND,
                                CD_ENDERECO_DESTINO,
                                CD_FUNCIONARIO,
                                DT_ADDROW,
                                ID_AREA_AVERIA,
                                NU_TRANSACCION) 
                            VALUES (
                                :nroEtiqueta,
                                :nroSecuencia,
                                :nroSecDetalle,
                                :ubicacionOrigen,
                                :empresa,
                                :producto,
                                :faixa,
                                :identificador,
                                :situacion,
                                :cantidad,
                                :vencimiento,
                                :averia,
                                :idInventario,
                                :fechaUltInventario,
                                :ctrlCalidad,
                                :ubicacionDestino,
                                :funcionario,
                                :dtAddrow,
                                :idAreaAveria,
                                :nuTransaccion)";

            await _dapper.ExecuteAsync(connection, sql, detalles, transaction: tran);
        }

        public virtual async Task BulkUpdateDetallePalletTransferencia(DbConnection connection, DbTransaction tran, List<object> detalles)
        {
            string sql = @"UPDATE T_DET_PALLET_TRANSFERENCIA 
                           SET CD_SITUACAO = :situacion, 
                           DT_UPDROW = :fechaModificacion,
                           NU_TRANSACCION = :nuTransaccion
                           WHERE NU_ETIQUETA = :nroEtiqueta 
                           AND NU_SEC_ETIQUETA = :nroSecuencia
                           AND NU_SEC_DETALLE = :nroSecDetalle";

            await _dapper.ExecuteAsync(connection, sql, detalles, transaction: tran);
        }

        public virtual object GetNewDetallePalletEntity(ITransferenciaStockServiceContext serviceContext, TransferenciaStock transferencia, Stock stock, decimal nroEtiqueta, int nroSecDetalle, long nuTransaccionCarga)
        {
            return new
            {
                nroEtiqueta = nroEtiqueta,
                nroSecuencia = 0,
                nroSecDetalle = nroSecDetalle,
                ubicacionOrigen = transferencia.Ubicacion,
                empresa = transferencia.Empresa,
                producto = transferencia.Producto,
                faixa = transferencia.Faixa,
                identificador = transferencia.Identificador,
                situacion = SituacionDb.EnTransferencia,
                cantidad = transferencia.Cantidad,
                vencimiento = stock.Vencimiento,
                averia = stock.Averia,
                idInventario = stock.Inventario,
                fechaUltInventario = stock.FechaInventario,
                ctrlCalidad = stock.ControlCalidad,
                ubicacionDestino = transferencia.UbicacionDestino,
                funcionario = _userId,
                dtAddrow = DateTime.Now,
                idAreaAveria = "N",
                nuTransaccion = nuTransaccionCarga,
            };
        }

        public virtual object GetUpdateDetallePalletEntity(decimal nroEtiqueta, int nroSecDetalle, long nuTransaccionDescarga)
        {
            return new
            {
                nroEtiqueta = nroEtiqueta,
                nroSecuencia = 0,
                nroSecDetalle = nroSecDetalle,
                fechaModificacion = DateTime.Now,
                situacion = SituacionDb.TransferenciaRealizada,
                nuTransaccion = nuTransaccionDescarga,
            };
        }

        public virtual object GetNewPalletEntity(ITransferenciaStockServiceContext serviceContext, decimal nroEtiqueta, long nuTransaccionCarga)
        {
            return new
            {
                nroEtiqueta = nroEtiqueta,
                nroSecuencia = 0,
                situacion = SituacionDb.EnTransferencia,
                ubicacion = serviceContext.UbicacionEquipo,
                predio = serviceContext.GetPredioOperacion(),
                dtAddrow = DateTime.Now,
                modalidadUso = TipoEtiquetaModalidadUso.Transferencia,
                idExterno = nroEtiqueta,
                tipoEtiqueta = TipoEtiquetaTransferencia.Estandar,
                app = _cdAplicacion,
                nuTransaccion = nuTransaccionCarga
            };
        }

        public virtual EtiquetaTransferencia GetNewPalletEntity(string ubicacion, string predio, decimal nroEtiqueta, long nuTransaccionCarga)
        {
            return new EtiquetaTransferencia
            {
                NumeroEtiqueta = nroEtiqueta,
                NumeroSecEtiqueta = 0,
                Estado = SituacionDb.EnTransferencia,
                UbicacionReal = ubicacion,
                Predio = predio,
                FechaInsercion = DateTime.Now,
                TpModalidadUso = TipoEtiquetaModalidadUso.Transferencia,
                IdExternoEtiqueta = nroEtiqueta.ToString(),
                TipoEtiquetaTransferencia = TipoEtiquetaTransferencia.Estandar,
                AplicacionOrigen = _cdAplicacion,
                NumeroTransaccion = nuTransaccionCarga
            };
        }

        public virtual object GetUpdatePalletEntity(decimal nroEtiqueta, long nuTransaccionDescarga)
        {
            return new
            {
                nroEtiqueta = nroEtiqueta,
                nroSecuencia = 0,
                situacion = SituacionDb.TransferenciaRealizada,
                fechaModificacion = DateTime.Now,
                nuTransaccion = nuTransaccionDescarga
            };
        }

        public virtual Stock GetNewStockEntity(TransferenciaStock transferencia, Stock stockOrigen, long nuTransaccion, string ubicacionEquipo = null)
        {
            decimal reserva = 0;
            var vencimiento = stockOrigen.Vencimiento;
            var ubicacion = transferencia.UbicacionDestino;

            if (!string.IsNullOrEmpty(ubicacionEquipo))
            {
                ubicacion = ubicacionEquipo;
                reserva = transferencia.Cantidad;
                vencimiento = null;
            }

            return new Stock
            {
                Ubicacion = ubicacion,
                Empresa = transferencia.Empresa,
                Producto = transferencia.Producto,
                Faixa = transferencia.Faixa,
                Identificador = transferencia.Identificador,
                Cantidad = transferencia.Cantidad,
                ReservaSalida = reserva,
                CantidadTransitoEntrada = 0,
                Vencimiento = vencimiento,
                Averia = stockOrigen.Averia,
                Inventario = stockOrigen.Inventario,
                FechaInventario = stockOrigen.FechaInventario,
                ControlCalidad = stockOrigen.ControlCalidad,
                FechaModificacion = DateTime.Now,
                NumeroTransaccion = nuTransaccion,
            };
        }

        public virtual Stock GetUpdateStockEntity(TransferenciaStock transferencia, string ubicacion, long nuTransaccion, DateTime? vencimiento = null)
        {
            return new Stock
            {
                Ubicacion = ubicacion,
                Empresa = transferencia.Empresa,
                Producto = transferencia.Producto,
                Faixa = transferencia.Faixa,
                Identificador = transferencia.Identificador,
                Cantidad = transferencia.Cantidad,
                Vencimiento = vencimiento,
                FechaModificacion = DateTime.Now,
                NumeroTransaccion = nuTransaccion,
            };
        }

        public virtual decimal GetNuEtiqueta(DbConnection connection, DbTransaction tran)
        {
            decimal nuEtiqueta = _dapper.GetNextSequenceValue<decimal>(connection, Secuencias.S_TRAN_NRO, tran);

            while (true)
            {
                if (!EtiquetaEnUso(connection, tran, nuEtiqueta))
                    break;
                else
                    nuEtiqueta = _dapper.GetNextSequenceValue<decimal>(connection, Secuencias.S_TRAN_NRO, tran);
            }

            return nuEtiqueta;
        }

        public virtual bool EtiquetaEnUso(DbConnection connection, DbTransaction tran, decimal nuEtiqueta)
        {
            string sql = @"SELECT NU_ETIQUETA FROM T_PALLET_TRANSFERENCIA 
                           WHERE NU_ETIQUETA = :nuEtiqueta AND NU_SEC_ETIQUETA = 0 ";

            var result = _dapper.Query<decimal?>(connection, sql, param: new { nuEtiqueta = nuEtiqueta }, transaction: tran)?.FirstOrDefault();

            return result != null;
        }

        public virtual async Task<long> CreateTransaction(string descripcion, DbConnection connection, DbTransaction tran)
        {
            var transaccionRepository = new TransaccionRepository(_context, _cdAplicacion, _userId, _dapper);
            return await transaccionRepository.CreateTransaction(descripcion, connection, tran, _cdAplicacion, _userId);
        }

        public virtual async Task TransferirStockAutomatismo(List<TransferenciaStock> transferencias, ITransferenciaStockServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    var bulkContext = await GetBulkOperationAutomatismoContext(transferencias, context, connection, tran);

                    await BulkUpdateStockBaja(connection, tran, bulkContext.UpdateStockOrigenBaja.Values);

                    await BulkUpdateStockAlta(connection, tran, bulkContext.UpdateStockDestinoAlta.Values, updateReserva: false, updateVencimiento: true);
                    await BulkInsertStock(connection, tran, bulkContext.NewStockDestino.Values);

                    tran.Commit();
                }
            }
        }

        public virtual async Task<TransferenciaBulkOperationContext> GetBulkOperationAutomatismoContext(List<TransferenciaStock> transferencias, ITransferenciaStockServiceContext serviceContext, DbConnection connection, DbTransaction tran)
        {
            var nroSecDetalle = 0;
            var context = new TransferenciaBulkOperationContext();
            var ubicacionEquipo = serviceContext.UbicacionEquipo;

            var nuTransaccion = await CreateTransaction($"Carga de etiqueta", connection, tran);

            foreach (var tr in transferencias)
            {
                var stockOrigen = serviceContext.GetStock(tr.Ubicacion, tr.Producto, tr.Empresa, tr.Identificador, tr.Faixa);

                var keyStockOrigen = $"{tr.Ubicacion}.{tr.Empresa}.{tr.Producto}.{tr.Identificador}.{tr.Faixa.ToString("#.###")}";
                if (!context.UpdateStockOrigenBaja.ContainsKey(keyStockOrigen))
                    context.UpdateStockOrigenBaja[keyStockOrigen] = GetUpdateStockEntity(tr, tr.Ubicacion, nuTransaccion);
                else
                    context.UpdateStockOrigenBaja[keyStockOrigen].Cantidad += tr.Cantidad;


                var keyStockDestino = $"{tr.UbicacionDestino}.{tr.Empresa}.{tr.Producto}.{tr.Identificador}.{tr.Faixa.ToString("#.###")}";
                var stockDestino = serviceContext.GetStock(tr.UbicacionDestino, tr.Producto, tr.Empresa, tr.Identificador, tr.Faixa);
                if (stockDestino != null)
                {
                    var vencimiento = (stockOrigen.Vencimiento < stockDestino.Vencimiento) ? stockOrigen.Vencimiento : stockDestino.Vencimiento;

                    if (!context.UpdateStockDestinoAlta.ContainsKey(keyStockDestino))
                        context.UpdateStockDestinoAlta[keyStockDestino] = GetUpdateStockEntity(tr, tr.UbicacionDestino, nuTransaccion, vencimiento);
                    else
                        context.UpdateStockDestinoAlta[keyStockDestino].Cantidad += tr.Cantidad;
                }
                else
                {
                    if (!context.NewStockDestino.ContainsKey(keyStockDestino))
                        context.NewStockDestino[keyStockDestino] = GetNewStockEntity(tr, stockOrigen, nuTransaccion);
                    else
                        context.NewStockDestino[keyStockDestino].Cantidad += tr.Cantidad;
                }

                nroSecDetalle++;
            }

            return context;
        }

        #endregion

        #region Mesa Clasificación

        public virtual void MovilizarStock(Stock stock, decimal cantidad)
        {
            var sql = @"
                UPDATE T_STOCK
                SET QT_ESTOQUE = COALESCE(QT_ESTOQUE, 0) + :QT_PRODUTO,
                    QT_RESERVA_SAIDA = COALESCE(QT_RESERVA_SAIDA, 0) + :QT_PRODUTO,
                    DT_FABRICACAO = :DT_FABRICACAO,
                    DT_UPDROW = :DT_UPDROW,
                    NU_TRANSACCION = :NU_TRANSACCION
                WHERE CD_ENDERECO = :CD_ENDERECO
                    AND CD_PRODUTO = :CD_PRODUTO
                    AND CD_FAIXA = :CD_FAIXA
                    AND NU_IDENTIFICADOR = :NU_IDENTIFICADOR
                    AND CD_EMPRESA = :CD_EMPRESA
            ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, new
            {
                QT_PRODUTO = cantidad,
                DT_FABRICACAO = stock.Vencimiento,
                DT_UPDROW = DateTime.Now,
                NU_TRANSACCION = stock.NumeroTransaccion,
                CD_ENDERECO = stock.Ubicacion,
                CD_EMPRESA = stock.Empresa,
                CD_PRODUTO = stock.Producto,
                CD_FAIXA = stock.Faixa,
                NU_IDENTIFICADOR = stock.Identificador,
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void MovilizarStockEtiquetaLote(int nuEtiquetaLote, string ubicacionDestino, long nuTransaccion)
        {
            BajaStockUbicacionEtiquetaLote(nuEtiquetaLote, nuTransaccion);
            AltaStockUbicacionDestino(nuEtiquetaLote, ubicacionDestino, nuTransaccion);
        }

        public virtual void AltaStockUbicacionDestino(int nuEtiquetaLote, string ubicacionDestino, long nuTransaccion)
        {
            var param = new
            {
                CD_ENDERECO = ubicacionDestino,
                NU_ETIQUETA_LOTE = nuEtiquetaLote,
                NU_TRANSACCION = nuTransaccion,
                DT_UPDROW = DateTime.Now,
            };

            var alias = "st";
            var from = @"
                T_STOCK st
                INNER JOIN (
                    SELECT 
                        d.CD_EMPRESA,
                        d.CD_PRODUTO,
                        d.CD_FAIXA,
                        d.NU_IDENTIFICADOR,
                        SUM(d.QT_PRODUTO) QT_PRODUTO
                    FROM T_DET_ETIQUETA_LOTE d
                    INNER JOIN T_ETIQUETA_LOTE e ON e.NU_ETIQUETA_LOTE = d.NU_ETIQUETA_LOTE
                    WHERE e.NU_ETIQUETA_LOTE = :NU_ETIQUETA_LOTE
                    GROUP BY d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR
                ) cc ON 
                cc.CD_EMPRESA = st.CD_EMPRESA AND
                cc.CD_PRODUTO = st.CD_PRODUTO AND
                cc.CD_FAIXA = st.CD_FAIXA AND
                cc.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR ";

            var set = @"
                QT_ESTOQUE = QT_ESTOQUE + QT_PRODUTO,
                QT_RESERVA_SAIDA = QT_RESERVA_SAIDA + QT_PRODUTO,
                NU_TRANSACCION = :NU_TRANSACCION,
                DT_UPDROW = :DT_UPDROW ";

            var where = "CD_ENDERECO = :CD_ENDERECO";

            _dapper.ExecuteUpdate(_context.Database.GetDbConnection(), alias, from, set, where, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            var sqlInsertNew = @"
                INSERT INTO T_STOCK (CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR, DT_FABRICACAO, DT_UPDROW, ID_AVERIA, ID_INVENTARIO, ID_CTRL_CALIDAD, QT_ESTOQUE, QT_RESERVA_SAIDA, QT_TRANSITO_ENTRADA, NU_TRANSACCION)
                SELECT :CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR, d.DT_FABRICACAO, :DT_UPDROW, 'N', 'R', CASE WHEN cc.NU_ETIQUETA IS NULL THEN 'C' ELSE 'P' END, d.QT_PRODUTO, d.QT_PRODUTO, 0, :NU_TRANSACCION
                FROM T_DET_ETIQUETA_LOTE d
                LEFT JOIN T_STOCK st ON st.CD_ENDERECO = :CD_ENDERECO
                    AND st.CD_EMPRESA = d.CD_EMPRESA
                    AND st.CD_PRODUTO = d.CD_PRODUTO
                    AND st.CD_FAIXA = d.CD_FAIXA
                    AND st.NU_IDENTIFICADOR = d.NU_IDENTIFICADOR
                LEFT JOIN T_CTR_CALIDAD_PENDIENTE cc ON cc.NU_ETIQUETA = d.NU_ETIQUETA_LOTE
                    AND st.CD_EMPRESA = d.CD_EMPRESA
                    AND st.CD_PRODUTO = d.CD_PRODUTO
                    AND st.CD_FAIXA = d.CD_FAIXA
                    AND st.NU_IDENTIFICADOR = d.NU_IDENTIFICADOR
                    AND cc.ID_ACEPTADO = 'N'
                WHERE st.CD_ENDERECO IS NULL 
                    AND d.NU_ETIQUETA_LOTE = :NU_ETIQUETA_LOTE
                ";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BajaStockUbicacionEtiquetaLote(int nuEtiquetaLote, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                NU_ETIQUETA_LOTE = nuEtiquetaLote,
                NU_TRANSACCION = nuTransaccion,
                DT_UPDROW = DateTime.Now,
            });

            var alias = "st";
            var from = @"
                T_STOCK st
                INNER JOIN (
                    SELECT 
                        e.CD_ENDERECO,
                        d.CD_EMPRESA,
                        d.CD_PRODUTO,
                        d.CD_FAIXA,
                        d.NU_IDENTIFICADOR,
                        SUM(d.QT_PRODUTO) QT_PRODUTO
                    FROM T_DET_ETIQUETA_LOTE d
                    INNER JOIN T_ETIQUETA_LOTE e ON e.NU_ETIQUETA_LOTE = d.NU_ETIQUETA_LOTE
                    WHERE e.NU_ETIQUETA_LOTE = :NU_ETIQUETA_LOTE
                    GROUP BY e.CD_ENDERECO, d.CD_EMPRESA, d.CD_PRODUTO, d.CD_FAIXA, d.NU_IDENTIFICADOR
                ) cc ON 
                cc.CD_ENDERECO = st.CD_ENDERECO AND
                cc.CD_PRODUTO = st.CD_PRODUTO AND
                cc.CD_FAIXA = st.CD_FAIXA AND
                cc.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR AND
                cc.CD_EMPRESA = st.CD_EMPRESA ";
            var set = @"
                QT_ESTOQUE = QT_ESTOQUE - QT_PRODUTO,
                QT_RESERVA_SAIDA = QT_RESERVA_SAIDA - QT_PRODUTO,
                NU_TRANSACCION = :NU_TRANSACCION,
                DT_UPDROW = :DT_UPDROW ";
            var where = "";

            _dapper.ExecuteUpdate(_context.Database.GetDbConnection(), alias, from, set, where, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion

        #region Produccion

        public virtual void UpdateBajaStock(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                Transaccion = nuTransaccion,
            });

            var alias = "st";
            var from = $@"
                T_STOCK st
                INNER JOIN (
                    SELECT 
                        st.CD_ENDERECO,
                        st.CD_EMPRESA,
                        st.CD_PRODUTO,
                        st.CD_FAIXA,
                        st.NU_IDENTIFICADOR,
                        max(st.QT_ESTOQUE) QT_EXPULSAR
                    FROM  T_STOCK_TEMP  st
                    GROUP by  st.CD_ENDERECO,
                        st.CD_EMPRESA,
                        st.CD_PRODUTO,
                        st.CD_FAIXA,
                        st.NU_IDENTIFICADOR
                ) s ON s.CD_ENDERECO = st.CD_ENDERECO AND
                     s.CD_PRODUTO = st.CD_PRODUTO AND
                     s.CD_FAIXA = st.CD_FAIXA AND
                     s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR AND
                     s.CD_EMPRESA = st.CD_EMPRESA ";
            var set = @"
                NU_TRANSACCION = :Transaccion,
                QT_ESTOQUE = QT_ESTOQUE - QT_EXPULSAR";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: param, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void UpdateAltaStock(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                Transaccion = nuTransaccion,
            });

            var alias = "st";
            var from = $@"
                T_STOCK st
                INNER JOIN (
                    SELECT 
                        st.CD_ENDERECO_DEST,
                        st.CD_EMPRESA,
                        st.CD_PRODUTO,
                        st.CD_FAIXA,
                        st.NU_IDENTIFICADOR,
                        max(st.QT_ESTOQUE) QT_EXPULSAR,
                        min(CASE WHEN s.DT_FABRICACAO IS NULL THEN st.DT_FABRICACAO WHEN  st.DT_FABRICACAO > s.DT_FABRICACAO  THEN s.DT_FABRICACAO ELSE st.DT_FABRICACAO END) DT_FABRICACAO_TEMP
                    FROM  T_STOCK_TEMP  st 
                    INNER JOIN T_STOCK s ON s.CD_ENDERECO = st.CD_ENDERECO_DEST AND
                     s.CD_EMPRESA = st.CD_EMPRESA AND
                     s.CD_PRODUTO = st.CD_PRODUTO AND
                     s.CD_FAIXA = st.CD_FAIXA AND
                     s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR
                    GROUP by  st.CD_ENDERECO_DEST,
                        st.CD_EMPRESA,
                        st.CD_PRODUTO,
                        st.CD_FAIXA,
                        st.NU_IDENTIFICADOR
                ) s ON s.CD_ENDERECO_DEST = st.CD_ENDERECO AND
                     s.CD_PRODUTO = st.CD_PRODUTO AND
                     s.CD_FAIXA = st.CD_FAIXA AND
                     s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR AND
                     s.CD_EMPRESA = st.CD_EMPRESA ";

            var set = @"
                NU_TRANSACCION = :Transaccion,
                QT_ESTOQUE = QT_ESTOQUE + QT_EXPULSAR,
                DT_FABRICACAO = DT_FABRICACAO_TEMP";

            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: param, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void UpdateAltaStockEquipamientos(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new DynamicParameters(new
            {
                Transaccion = nuTransaccion,
            });

            var alias = "st";
            var from = $@"
                T_STOCK st
                INNER JOIN (
                    SELECT 
                        st.CD_ENDERECO_DEST,
                        st.CD_EMPRESA,
                        st.CD_PRODUTO,
                        st.CD_FAIXA,
                        st.NU_IDENTIFICADOR,
                        max(st.QT_ESTOQUE) QT_EXPULSAR
                    FROM  T_STOCK_TEMP  st 
                    INNER JOIN T_STOCK s ON s.CD_ENDERECO = st.CD_ENDERECO_DEST AND
                     s.CD_EMPRESA = st.CD_EMPRESA AND
                     s.CD_PRODUTO = st.CD_PRODUTO AND
                     s.CD_FAIXA = st.CD_FAIXA AND
                     s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR
                    GROUP by  st.CD_ENDERECO_DEST,
                        st.CD_EMPRESA,
                        st.CD_PRODUTO,
                        st.CD_FAIXA,
                        st.NU_IDENTIFICADOR
                ) s ON s.CD_ENDERECO_DEST = st.CD_ENDERECO AND
                     s.CD_PRODUTO = st.CD_PRODUTO AND
                     s.CD_FAIXA = st.CD_FAIXA AND
                     s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR AND
                     s.CD_EMPRESA = st.CD_EMPRESA ";

            var set = @"
                NU_TRANSACCION = :Transaccion,
                QT_ESTOQUE = QT_ESTOQUE + QT_EXPULSAR,
                QT_RESERVA_SAIDA = QT_RESERVA_SAIDA + QT_EXPULSAR";

            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: param, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void InsertStock(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            string sql = $@"
                INSERT INTO T_STOCK (
                    CD_ENDERECO, 
                    CD_EMPRESA, 
                    CD_PRODUTO, 
                    CD_FAIXA, 
                    NU_IDENTIFICADOR, 
                    QT_ESTOQUE, 
                    QT_RESERVA_SAIDA, 
                    QT_TRANSITO_ENTRADA, 
                    DT_UPDROW, 
                    DT_INVENTARIO,
                    ID_AVERIA,
                    ID_INVENTARIO,
                    ID_CTRL_CALIDAD,
                    NU_TRANSACCION,
                    DT_FABRICACAO
                )
                SELECT 
                    st.CD_ENDERECO_DEST, 
                    st.CD_EMPRESA, 
                    st.CD_PRODUTO, 
                    st.CD_FAIXA, 
                    st.NU_IDENTIFICADOR,
                    sum(st.QT_ESTOQUE),                    
                    0, 
                    0,
                    :FechaModificacion,
                    :FechaModificacion,
                    'N',
                    'R',
                    'C',
                    :Transaccion,
                    st.DT_FABRICACAO
                FROM T_STOCK_TEMP st                    
                LEFT JOIN T_STOCK sto ON st.CD_ENDERECO_DEST  = sto.CD_ENDERECO
                    AND st.CD_EMPRESA = sto.CD_EMPRESA 
                    AND st.CD_PRODUTO = sto.CD_PRODUTO 
                    AND st.CD_FAIXA = sto.CD_FAIXA 
                    AND st.NU_IDENTIFICADOR = sto.NU_IDENTIFICADOR
                WHERE sto.CD_PRODUTO IS NULL 
                GROUP BY 
                    st.CD_ENDERECO_DEST, 
                    st.CD_EMPRESA, 
                    st.CD_PRODUTO, 
                    st.CD_FAIXA, 
                    st.NU_IDENTIFICADOR,
                    st.DT_FABRICACAO";

            _dapper.Execute(connection, sql, new
            {
                Transaccion = nuTransaccion,
                FechaModificacion = DateTime.Now,
            }, transaction: tran);
        }

        public virtual void InsertStockEquipamientos(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            string sql = $@"
                INSERT INTO T_STOCK (
                    CD_ENDERECO, 
                    CD_EMPRESA, 
                    CD_PRODUTO, 
                    CD_FAIXA, 
                    NU_IDENTIFICADOR, 
                    QT_ESTOQUE, 
                    QT_RESERVA_SAIDA, 
                    QT_TRANSITO_ENTRADA, 
                    DT_UPDROW, 
                    DT_INVENTARIO,
                    ID_AVERIA,
                    ID_INVENTARIO,
                    ID_CTRL_CALIDAD,
                    NU_TRANSACCION
                )
                SELECT 
                    st.CD_ENDERECO_DEST, 
                    st.CD_EMPRESA, 
                    st.CD_PRODUTO, 
                    st.CD_FAIXA, 
                    st.NU_IDENTIFICADOR,
                    sum(st.QT_ESTOQUE),                    
                    sum(st.QT_ESTOQUE), 
                    0,
                    :FechaModificacion,
                    :FechaModificacion,
                    'N',
                    'R',
                    'C',
                    :Transaccion
                FROM T_STOCK_TEMP st                    
                LEFT JOIN T_STOCK sto ON st.CD_ENDERECO_DEST = sto.CD_ENDERECO  
                    AND st.CD_EMPRESA = sto.CD_EMPRESA 
                    AND st.CD_PRODUTO = sto.CD_PRODUTO 
                    AND st.CD_FAIXA = sto.CD_FAIXA 
                    AND st.NU_IDENTIFICADOR = sto.NU_IDENTIFICADOR
                WHERE sto.CD_PRODUTO IS NULL 
                GROUP BY 
                    st.CD_ENDERECO_DEST, 
                    st.CD_EMPRESA, 
                    st.CD_PRODUTO, 
                    st.CD_FAIXA, 
                    st.NU_IDENTIFICADOR";

            _dapper.Execute(connection, sql, new
            {
                Transaccion = nuTransaccion,
                FechaModificacion = DateTime.Now,
            }, transaction: tran);
        }

        public virtual void BulkInsertDetallePalletTransferencia(DbConnection connection, DbTransaction tran, long nuTransaccion, int userId, EtiquetaTransferencia palletTransferencia, string nroIngresoProduccion)
        {
            string sql = @"INSERT INTO T_DET_PALLET_TRANSFERENCIA 
                                (NU_ETIQUETA,
                                NU_SEC_ETIQUETA,
                                NU_SEC_DETALLE,
                                CD_ENDERECO_ORIGEN,
                                CD_EMPRESA,
                                CD_PRODUTO,
                                CD_FAIXA,
                                NU_IDENTIFICADOR,
                                CD_SITUACAO,
                                QT_PRODUTO,
                                DT_FABRICACAO,
                                ID_AVERIA,
                                DT_ULT_INVENTARIO,
                                ID_CTRL_CALI_PEND,
                                CD_FUNCIONARIO,
                                DT_ADDROW,
                                ID_AREA_AVERIA,
                                NU_TRANSACCION,
                                VL_METADATA) 
							SELECT :NumeroEtiqueta,
								:NumeroSecEtiqueta,
								st.IDROW,
								st.CD_ENDERECO,
								st.CD_EMPRESA,
								st.CD_PRODUTO,
								st.CD_FAIXA,
								st.NU_IDENTIFICADOR,
								:Situacion,
								st.QT_ESTOQUE,
								s.DT_FABRICACAO,
								s.ID_AVERIA,
								s.DT_INVENTARIO,
								s.ID_CTRL_CALIDAD,
								:userId,
								:FechaCreacion,
								:idAreaAveria,
								:Transaccion,
                                :NroIngresoProduccion
							FROM T_STOCK_TEMP st
							LEFT JOIN T_STOCK s on s.CD_ENDERECO = st.CD_ENDERECO AND
								 s.CD_EMPRESA = st.CD_EMPRESA AND
								 s.CD_PRODUTO = st.CD_PRODUTO AND
								 s.CD_FAIXA = st.CD_FAIXA AND
								 s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR";

            _dapper.Execute(connection, sql, new
            {
                Transaccion = nuTransaccion,
                Situacion = SituacionDb.EnTransferencia,
                idAreaAveria = "N",
                NumeroEtiqueta = palletTransferencia.NumeroEtiqueta,
                NumeroSecEtiqueta = palletTransferencia.NumeroSecEtiqueta,
                userId = userId,
                FechaCreacion = DateTime.Now,
                NroIngresoProduccion = nroIngresoProduccion
            }, transaction: tran);
        }

        public virtual void InsertPalletTransferencia(DbConnection connection, DbTransaction tran, EtiquetaTransferencia pallet)
        {
            string sql = @"INSERT INTO T_PALLET_TRANSFERENCIA 
                            (NU_ETIQUETA,
                            NU_SEC_ETIQUETA,
                            CD_SITUACAO,
                            CD_ENDERECO_REAL,
                            DT_ADDROW,
                            CD_APLICACAO_ORIGEN,
                            NU_PREDIO,
                            TP_MODALIDAD_USO,
                            NU_TRANSACCION,
                            ID_EXTERNO_ETIQUETA,
                            TP_ETIQUETA_TRANSFERENCIA) 
                        VALUES 
                            (:NumeroEtiqueta,
                            :NumeroSecEtiqueta,
                            :Estado,
                            :UbicacionReal, 
                            :FechaInsercion,
                            :AplicacionOrigen,
                            :Predio,
                            :TpModalidadUso,
                            :NumeroTransaccion,
                            :IdExternoEtiqueta,
                            :TipoEtiquetaTransferencia)";

            _dapper.Execute(connection, sql, pallet, transaction: tran);
        }

        #endregion

        public virtual void UpdateStock(string ubicacion, int empresa, string producto, decimal faixa, string identificador, decimal cantidad, bool alta, bool modificarReserva)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var sqlReserva = modificarReserva ? $"QT_RESERVA_SAIDA = QT_RESERVA_SAIDA {(alta ? "+" : "-")} :Cantidad," : "";
            string sql = $@"UPDATE T_STOCK 
                    SET 
                        QT_ESTOQUE = QT_ESTOQUE {(alta ? "+" : "-")} :Cantidad,
                        {sqlReserva}
                        NU_TRANSACCION = :NuTransaccion,
                        DT_UPDROW = :FechaModificacion 
                    WHERE CD_ENDERECO = :Ubicacion 
                        AND CD_PRODUTO = :Producto 
                        AND CD_FAIXA = :Faixa 
                        AND NU_IDENTIFICADOR = :Identificador
                        AND CD_EMPRESA = :Empresa ";

            _dapper.Execute(connection, sql, param: new
            {
                Ubicacion = ubicacion,
                Empresa = empresa,
                Producto = producto,
                Faixa = faixa,
                Identificador = identificador,
                Cantidad = cantidad,
                NuTransaccion = _context.GetTransactionNumber(),
                FechaModificacion = DateTime.Now
            }, transaction: tran);

        }

        public virtual void UpdateOrInsertStock(string ubicacion, int empresa, string producto, decimal faixa, string identificador, decimal cantidad, bool alta, bool modificarReserva)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var stock = GetStock(ubicacion, empresa, producto, faixa, identificador, connection, tran);

            if (stock != null)
            {
                var sqlReserva = modificarReserva ? $"QT_RESERVA_SAIDA = QT_RESERVA_SAIDA {(alta ? "+" : "-")} :Cantidad," : "";
                string sql = $@"UPDATE T_STOCK 
                    SET 
                        QT_ESTOQUE = QT_ESTOQUE {(alta ? "+" : "-")} :Cantidad,
                        {sqlReserva}
                        NU_TRANSACCION = :NuTransaccion,
                        DT_UPDROW = :FechaModificacion 
                    WHERE CD_ENDERECO = :Ubicacion 
                        AND CD_PRODUTO = :Producto 
                        AND CD_FAIXA = :Faixa 
                        AND NU_IDENTIFICADOR = :Identificador
                        AND CD_EMPRESA = :Empresa ";

                _dapper.Execute(connection, sql, param: new
                {
                    Ubicacion = ubicacion,
                    Empresa = empresa,
                    Producto = producto,
                    Faixa = faixa,
                    Identificador = identificador,
                    Cantidad = cantidad,
                    NuTransaccion = _context.GetTransactionNumber(),
                    FechaModificacion = DateTime.Now
                }, transaction: tran);
            }
            else
            {
                string sql = @"INSERT INTO T_STOCK 
                            (CD_ENDERECO,
                            CD_EMPRESA,
                            CD_PRODUTO,
                            CD_FAIXA,
                            NU_IDENTIFICADOR,
                            QT_ESTOQUE,
                            QT_RESERVA_SAIDA,
                            QT_TRANSITO_ENTRADA,
                            ID_AVERIA,
                            ID_INVENTARIO,
                            DT_INVENTARIO,
                            ID_CTRL_CALIDAD,
                            DT_UPDROW,
                            NU_TRANSACCION) 
                        VALUES 
                            (:Ubicacion,
                            :Empresa,
                            :Producto,
                            :Faixa,
                            :Identificador,
                            :Cantidad,
                            :Cantidad, 
                            :CantidadTransitoEntrada,
                            :Averia, 
                            :Inventario,
                            :FechaInventario,
                            :ControlCalidad,
                            :FechaModificacion,
                            :NumeroTransaccion)";

                _dapper.Execute(connection, sql, param: new
                {
                    Ubicacion = ubicacion,
                    Empresa = empresa,
                    Producto = producto,
                    Faixa = faixa,
                    Identificador = identificador,
                    Cantidad = cantidad,
                    CantidadTransitoEntrada = 0,
                    Averia = "N",
                    Inventario = "R",
                    FechaInventario = DateTime.Now,
                    ControlCalidad = EstadoControlCalidad.Controlado,
                    FechaModificacion = DateTime.Now,
                    NumeroTransaccion = _context.GetTransactionNumber(),
                }, transaction: tran);
            }
        }

        #endregion
    }
}
