using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class UbicacionPickingProductoRepository
    {
        protected WISDB _context;
        protected string application;
        protected int userId;
        protected readonly UbicacionPickingProductoMapper _mapper;
        protected readonly IDapper _dapper;

        public UbicacionPickingProductoRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this.application = application;
            this.userId = userId;
            this._mapper = new UbicacionPickingProductoMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyUbicacionPickingProducto(string idUbicacion)
        {
            return _context.T_PICKING_PRODUTO
                .Any(x => x.CD_ENDERECO_SEPARACAO == idUbicacion);
        }

        public virtual bool AnyUbicacionPickingOtroProducto(string idUbicacion, int empresa, string producto)
        {
            return _context.T_PICKING_PRODUTO
               .Any(x => x.CD_ENDERECO_SEPARACAO == idUbicacion
                    && (x.CD_EMPRESA != empresa || x.CD_PRODUTO != producto));
        }

        public virtual bool AnyUbicacionProductoPadronPredio(string producto, int empresa, int padron, string predio)
        {
            return this._context.T_PICKING_PRODUTO
                .Any(x => x.CD_PRODUTO == producto
                    && x.CD_EMPRESA == empresa
                    && x.NU_PREDIO == predio
                    && x.QT_PADRAO_PICKING == padron);
        }

        public virtual bool AnyUbicacionProductoPadronPrioridad(string producto, int empresa, int padron, string predio, int prioridad)
        {
            return this._context.T_PICKING_PRODUTO
                .Any(x => x.CD_PRODUTO == producto
                    && x.CD_EMPRESA == empresa
                    && x.NU_PREDIO == predio
                    && x.QT_PADRAO_PICKING == padron
                    && x.NU_PRIORIDAD == prioridad);
        }

        public virtual bool AnyUbicacionPickingProducto(string idUbicacion, int empresa, string producto, decimal faixa)
        {
            return _context.T_PICKING_PRODUTO
               .Any(x => x.CD_ENDERECO_SEPARACAO == idUbicacion
                    && x.CD_EMPRESA == empresa && x.CD_PRODUTO == producto && x.CD_FAIXA == faixa);
        }

        #endregion

        #region Get

        public virtual UbicacionPickingProducto GetUbicacionPickingProducto(int numeroUbicacionPicking)
        {
            return this._mapper.MapToObject(this._context.T_PICKING_PRODUTO.FirstOrDefault(x => x.NU_SEC_PICKING_PRODUTO == numeroUbicacionPicking));
        }

        public virtual UbicacionPickingProducto GetUbicacionPickingProducto(int idEmpresa, string codigoProducto, decimal faixa, decimal padron, string predio, string ubicacion, int prioridad)
        {
            var entity = this._context.T_PICKING_PRODUTO
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_EMPRESA == idEmpresa
                    && d.CD_PRODUTO == codigoProducto
                    && d.CD_FAIXA == faixa
                    && d.QT_PADRAO_PICKING == padron
                    && d.NU_PREDIO == predio
                    && d.CD_ENDERECO_SEPARACAO == ubicacion
                    && d.NU_PRIORIDAD == prioridad);

            return this._mapper.MapToObject(entity);
        }

        public virtual List<UbicacionPickingProducto> GetUbicacionesPicking()
        {
            var ubicaciones = new List<UbicacionPickingProducto>();

            var entities = this._context.T_PICKING_PRODUTO
                .AsNoTracking();

            foreach (var entity in entities)
            {
                ubicaciones.Add(this._mapper.MapToObject(entity));
            }

            return ubicaciones;
        }

        #endregion

        #region Add

        public virtual void AddUbicacionPicking(UbicacionPickingProducto ubicacionPicking)
        {
            ubicacionPicking.Id = this._context.GetNextSequenceValueInt(_dapper, "S_SEC_PICKING_PRODUTO");
            this._context.T_PICKING_PRODUTO.Add(this._mapper.MapToEntity(ubicacionPicking));


        }

        #endregion

        #region Update

        public virtual void ActualizarUbicacionPicking(UbicacionPickingProducto ubicacionPicking)
        {
            T_PICKING_PRODUTO entity = this._mapper.MapToEntity(ubicacionPicking);

            T_PICKING_PRODUTO attachedEntity = _context.T_PICKING_PRODUTO.Local
                .FirstOrDefault(x => x.NU_SEC_PICKING_PRODUTO == entity.NU_SEC_PICKING_PRODUTO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_PICKING_PRODUTO.Attach(entity);
                _context.Entry<T_PICKING_PRODUTO>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void BorrarUbicacionPicking(UbicacionPickingProducto ubicacionPicking)
        {
            var entity = this._mapper.MapToEntity(ubicacionPicking);
            var attachedEntity = _context.T_PICKING_PRODUTO.Local
                .FirstOrDefault(x => x.NU_SEC_PICKING_PRODUTO == entity.NU_SEC_PICKING_PRODUTO);

            if (attachedEntity != null)
            {
                this._context.T_PICKING_PRODUTO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_PICKING_PRODUTO.Attach(entity);
                this._context.T_PICKING_PRODUTO.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<UbicacionPickingProducto> GetUbicacionPickingProducto(IEnumerable<UbicacionPickingProducto> ubicacion)
        {
            IEnumerable<UbicacionPickingProducto> resultado = new List<UbicacionPickingProducto>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_PRODUTO_TEMP (CD_ENDERECO_SEPARACAO, CD_EMPRESA, CD_PRODUTO) VALUES (:UbicacionSeparacion, :Empresa, :CodigoProducto)";
                    _dapper.Execute(connection, sql, ubicacion, transaction: tran);

                    sql = @"SELECT 
                                P.CD_ENDERECO_SEPARACAO AS UbicacionSeparacion,
                                P.CD_EMPRESA AS Empresa,
                                P.CD_PRODUTO AS CodigoProducto,
                                P.CD_FAIXA AS Faixa,
                                P.QT_ESTOQUE_MINIMO AS StockMinimo,
                                P.QT_ESTOQUE_MAXIMO AS StockMaximo,
                                P.QT_PADRAO_PICKING AS Padron,
                                P.QT_DESBORDE AS CantidadDesborde,
                                P.NU_PREDIO AS Predio,
                                P.QT_PADRON_DESBORDE AS CantidadPadronDesborde,
                                P.NU_PRIORIDAD AS Prioridad
                            FROM T_PICKING_PRODUTO P 
                            INNER JOIN T_PICKING_PRODUTO_TEMP T ON 
                                P.CD_ENDERECO_SEPARACAO = T.CD_ENDERECO_SEPARACAO AND 
                                P.CD_EMPRESA = T.CD_EMPRESA AND 
                                P.CD_PRODUTO = T.CD_PRODUTO";

                    resultado = _dapper.Query<UbicacionPickingProducto>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<UbicacionPickingProducto> GetUbicacionPickingProductoByKeys(IEnumerable<UbicacionPickingProducto> ubicacion)
        {
            IEnumerable<UbicacionPickingProducto> resultado = new List<UbicacionPickingProducto>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_PRODUTO_TEMP (CD_ENDERECO_SEPARACAO, CD_EMPRESA, CD_PRODUTO, QT_PADRAO_PICKING, NU_PRIORIDAD) VALUES (:UbicacionSeparacion, :Empresa, :CodigoProducto, :Padron, :Prioridad)";
                    _dapper.Execute(connection, sql, ubicacion, transaction: tran);

                    sql = @"SELECT 
                                P.CD_ENDERECO_SEPARACAO AS UbicacionSeparacion,
                                P.CD_EMPRESA AS Empresa,
                                P.CD_PRODUTO AS CodigoProducto,
                                P.CD_FAIXA AS Faixa,
                                P.QT_ESTOQUE_MINIMO AS StockMinimo,
                                P.QT_ESTOQUE_MAXIMO AS StockMaximo,
                                P.QT_PADRAO_PICKING AS Padron,
                                P.QT_DESBORDE AS CantidadDesborde,
                                P.NU_PREDIO AS Predio,
                                P.QT_PADRON_DESBORDE AS CantidadPadronDesborde,
                                P.NU_PRIORIDAD AS Prioridad
                            FROM T_PICKING_PRODUTO P 
                            INNER JOIN T_PICKING_PRODUTO_TEMP T ON 
                                P.CD_ENDERECO_SEPARACAO = T.CD_ENDERECO_SEPARACAO AND 
                                P.CD_EMPRESA = T.CD_EMPRESA AND 
                                P.CD_PRODUTO = T.CD_PRODUTO AND
                                P.QT_PADRAO_PICKING = T.QT_PADRAO_PICKING AND
                                P.NU_PRIORIDAD = T.NU_PRIORIDAD";

                    resultado = _dapper.Query<UbicacionPickingProducto>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<UbicacionPickingProducto> GetUbicacionPickingProductoWhithKeys(IEnumerable<UbicacionPickingProducto> ubicacion)
        {
            IEnumerable<UbicacionPickingProducto> resultado = new List<UbicacionPickingProducto>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PICKING_PRODUTO_TEMP (CD_ENDERECO_SEPARACAO, CD_EMPRESA, CD_PRODUTO) VALUES (:UbicacionSeparacion, :Empresa, :CodigoProducto)";
                    _dapper.Execute(connection, sql, ubicacion, transaction: tran);

                    sql = @"SELECT 
                                P.CD_ENDERECO_SEPARACAO AS UbicacionSeparacion,
                                P.CD_EMPRESA AS Empresa,
                                P.CD_PRODUTO AS CodigoProducto,
                                P.CD_FAIXA AS Faixa,
                                P.QT_ESTOQUE_MINIMO AS StockMinimo,
                                P.QT_ESTOQUE_MAXIMO AS StockMaximo,
                                P.QT_PADRAO_PICKING AS Padron,
                                P.QT_DESBORDE AS CantidadDesborde,
                                P.NU_PREDIO AS Predio,
                                P.QT_PADRON_DESBORDE AS CantidadPadronDesborde,
                                P.NU_PRIORIDAD AS Prioridad
                            FROM T_PICKING_PRODUTO P 
                            INNER JOIN T_PICKING_PRODUTO_TEMP T ON 
                                P.CD_ENDERECO_SEPARACAO = T.CD_ENDERECO_SEPARACAO AND 
                                P.CD_EMPRESA = T.CD_EMPRESA AND 
                                P.CD_PRODUTO = T.CD_PRODUTO";

                    resultado = _dapper.Query<UbicacionPickingProducto>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task AddUbicacionesPicking(List<UbicacionPickingProducto> ubicacionesPicking, PickingProductoServiceContext context, CancellationToken cancelToken = default)
        {
            await AddUbicacionesPicking(GetBulkOperationContext(ubicacionesPicking, context), cancelToken);
        }

        public virtual async Task AddUbicacionesPicking(PickingProductoBulkOperationContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertUbicacionesPicking(connection, tran, context.NewUbicacionesPicking);
                    await BulkUpdateUbicacionesPicking(connection, tran, context.UpdUbicacionesPicking);
                    await BulkDeleteUbicacionesPicking(connection, tran, context.DelUbicacionesPicking);

                    tran.Commit();
                }
            }
        }

        public virtual PickingProductoBulkOperationContext GetBulkOperationContext(List<UbicacionPickingProducto> ubicacionesPicking, PickingProductoServiceContext serviceContext)
        {
            var context = new PickingProductoBulkOperationContext();
            var newUbicacionPicking = new Dictionary<string, UbicacionPickingProducto>();

            foreach (var ubicacionPicking in ubicacionesPicking)
            {
                var keyPickingProducto = $"{ubicacionPicking.UbicacionSeparacion}.{ubicacionPicking.CodigoProducto}.{ubicacionPicking.Empresa}.{ubicacionPicking.Padron}.{ubicacionPicking.Prioridad}";

                newUbicacionPicking[keyPickingProducto] = ubicacionPicking;
            }

            foreach (var model in serviceContext.UbicacionPickingProducto.Values)
            {
                var keyPickingProducto = $"{model.UbicacionSeparacion}.{model.CodigoProducto}.{model.Empresa}.{model.Padron}.{model.Prioridad}";
                var ubicacionPicking = newUbicacionPicking[keyPickingProducto];

                newUbicacionPicking.Remove(keyPickingProducto);
                ubicacionPicking = Map(ubicacionPicking, model);

                if (ubicacionPicking.TipoOperacionId == "B")
                {
                    context.DelUbicacionesPicking.Add(new
                    {
                        Ubicacion = model.UbicacionSeparacion,
                        Producto = model.CodigoProducto,
                        Empresa = ubicacionPicking.Empresa,
                        Padron = ubicacionPicking.Padron,
                        Prioridad = ubicacionPicking.Prioridad,
                        transaccion = ubicacionPicking.NuTransaccion,
                    });
                }
                else if (ubicacionPicking.TipoOperacionId == "S")
                {
                    context.UpdUbicacionesPicking.Add(GetUbicacionPickingEntity(ubicacionPicking));
                }
            }

            foreach (var newUbicacion in newUbicacionPicking.Values)
            {
                context.NewUbicacionesPicking.Add(GetUbicacionPickingEntity(newUbicacion));
            }

            return context;
        }

        public virtual async Task BulkInsertUbicacionesPicking(DbConnection connection, DbTransaction tran, List<object> ubicaciones)
        {
            string sql = @"INSERT INTO T_PICKING_PRODUTO 
                    (CD_EMPRESA, CD_ENDERECO_SEPARACAO, CD_FAIXA, CD_PRODUTO, DT_ADDROW, NU_PREDIO,
                     QT_DESBORDE, QT_ESTOQUE_MAXIMO, QT_ESTOQUE_MINIMO, QT_PADRAO_PICKING, QT_PADRON_DESBORDE, TP_PICKING, NU_TRANSACCION,
                       CD_UNIDAD_CAJA_AUT, QT_UNIDAD_CAJA_AUT, FL_CONF_CD_BARRAS_AUT, NU_PRIORIDAD ) 
                   VALUES 
                    (:Empresa, :Ubicacion, :Faixa, :Producto, :FechaInsercion, :Predio, 
                      :Desborde, :StockMaximo, :StockMinimo, :PickingPadron, :PickingDesborde, :TipoPicking, :Transaccion,
                       :CodigoUnidadCajaAutomatismo, :CantidadUnidadCajaAutomatismo, :FlagConfirmarCodBarrasAutomatismo, :Prioridad)";

            await _dapper.ExecuteAsync(connection, sql, ubicaciones, transaction: tran);
        }

        public virtual async Task BulkUpdateUbicacionesPicking(DbConnection connection, DbTransaction tran, List<object> ubicaciones)
        {
            string sql = @"
                UPDATE T_PICKING_PRODUTO SET 
                    NU_TRANSACCION = :Transaccion,
                    QT_ESTOQUE_MINIMO = :StockMinimo,
                    QT_ESTOQUE_MAXIMO = :StockMaximo,
                    QT_DESBORDE = :Desborde,
                    CD_UNIDAD_CAJA_AUT = :CodigoUnidadCajaAutomatismo,
                    QT_UNIDAD_CAJA_AUT = :CantidadUnidadCajaAutomatismo,
                    FL_CONF_CD_BARRAS_AUT = :FlagConfirmarCodBarrasAutomatismo,
                    NU_PRIORIDAD = :Prioridad
                WHERE 
                    CD_ENDERECO_SEPARACAO = :Ubicacion AND 
                    CD_EMPRESA = :Empresa AND 
                    CD_PRODUTO = :Producto AND
                    QT_PADRAO_PICKING = :Padron AND
                    NU_PRIORIDAD = :Prioridad";

            await _dapper.ExecuteAsync(connection, sql, ubicaciones, transaction: tran);
        }

        public virtual async Task BulkDeleteUbicacionesPicking(DbConnection connection, DbTransaction tran, List<object> ubicaciones)
        {
            string sql = @"
            UPDATE T_PICKING_PRODUTO SET 
                NU_TRANSACCION = :transaccion,
                NU_TRANSACCION_DELETE = :transaccion
            WHERE 
                CD_ENDERECO_SEPARACAO = :Ubicacion AND 
                CD_EMPRESA = :Empresa AND 
                CD_PRODUTO = :Producto AND
                QT_PADRAO_PICKING = :Padron  AND
                NU_PRIORIDAD = :Prioridad";

            await _dapper.ExecuteAsync(connection, sql, ubicaciones, transaction: tran);

            sql = @"
                DELETE FROM T_PICKING_PRODUTO 
                WHERE 
                    CD_ENDERECO_SEPARACAO = :Ubicacion AND 
                    CD_EMPRESA = :Empresa AND 
                    CD_PRODUTO = :Producto AND
                    QT_PADRAO_PICKING = :Padron AND
                    NU_PRIORIDAD = :Prioridad";

            await _dapper.ExecuteAsync(connection, sql, ubicaciones, transaction: tran);
        }

        public static object GetUbicacionPickingEntity(UbicacionPickingProducto ubicacion)
        {
            return new
            {
                Empresa = ubicacion.Empresa,
                Ubicacion = ubicacion.UbicacionSeparacion,
                Faixa = ubicacion.Faixa,
                Producto = ubicacion.CodigoProducto,
                FechaInsercion = ubicacion.FechaInsercion ?? DateTime.Now,
                FechaModificacion = DateTime.Now,
                Predio = ubicacion.Predio,
                Transaccion = ubicacion.NuTransaccion,
                Desborde = ubicacion.CantidadDesborde,
                StockMaximo = ubicacion.StockMaximo,
                StockMinimo = ubicacion.StockMinimo,
                PickingPadron = ubicacion.Padron,
                PickingDesborde = ubicacion.CantidadPadronDesborde,
                TipoPicking = ubicacion.TipoPicking,
                CodigoUnidadCajaAutomatismo = ubicacion.CodigoUnidadCajaAutomatismo,
                CantidadUnidadCajaAutomatismo = ubicacion.CantidadUnidadCajaAutomatismo,
                FlagConfirmarCodBarrasAutomatismo = ubicacion.FlagConfirmarCodBarrasAutomatismo,
                Prioridad = ubicacion.Prioridad,
                Padron = ubicacion.Padron,
            };
        }

        public virtual UbicacionPickingProducto Map(UbicacionPickingProducto request, UbicacionPickingProducto model = null)
        {
            UbicacionPickingProducto ubicacionPicking = new UbicacionPickingProducto();

            ubicacionPicking.Id = request.Id;
            ubicacionPicking.Empresa = request.Empresa;
            ubicacionPicking.CodigoProducto = request.CodigoProducto;
            ubicacionPicking.Faixa = request.Faixa;
            ubicacionPicking.Padron = request.Padron;
            ubicacionPicking.UbicacionSeparacion = request.UbicacionSeparacion;
            ubicacionPicking.StockMinimo = request.StockMinimo ?? model?.StockMinimo;
            ubicacionPicking.StockMaximo = request.StockMaximo ?? model?.StockMaximo;
            ubicacionPicking.CantidadDesborde = request.CantidadDesborde ?? model?.CantidadDesborde;
            ubicacionPicking.TipoPicking = request.TipoPicking ?? model?.TipoPicking;
            ubicacionPicking.FechaInsercion = request.FechaInsercion ?? model?.FechaInsercion;
            ubicacionPicking.FechaModificacion = request.FechaModificacion ?? model?.FechaModificacion;
            ubicacionPicking.Predio = request.Predio ?? model?.Predio;
            ubicacionPicking.NuTransaccion = request.NuTransaccion ?? model?.NuTransaccion;
            ubicacionPicking.TipoOperacionId = request.TipoOperacionId ?? model?.TipoOperacionId;

            return ubicacionPicking;
        }

        #endregion
    }
}
