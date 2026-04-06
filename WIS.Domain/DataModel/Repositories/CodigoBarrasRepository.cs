using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.CodigoMultidato.Constants;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class CodigoBarrasRepository
    {
        protected readonly int _userId;
        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly string _cdAplicacion;
        protected readonly CodigoBarrasMapper _mapper;

        public CodigoBarrasRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new CodigoBarrasMapper();
            this._dapper = dapper;
        }

        #region Any

        #endregion

        #region Get

        public virtual CodigoBarras GetCodigoBarras(string producto, int empresa, int? tipoCodigo)
        {
            var cb = _context.T_CODIGO_BARRAS
                           .AsNoTracking()
                           .Where(x => x.CD_EMPRESA == empresa && x.CD_PRODUTO == producto &&
                           (tipoCodigo == null ? true : x.TP_CODIGO_BARRAS == tipoCodigo.Value))
                           .OrderBy(x => x.NU_PRIORIDADE_USO)
                           .FirstOrDefault();

            return cb != null ? _mapper.MapToObject(cb) : null;
        }

        public virtual CodigoBarras GetCodigoBarras(string barra, int empresa)
        {
            var codigoBarra = this._context.T_CODIGO_BARRAS
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_BARRAS == barra
                    && d.CD_EMPRESA == empresa);

            if (codigoBarra == null)
                return null;

            return this._mapper.MapToObject(codigoBarra);
        }

        public virtual int? GetFirstEmpresaWithCodigoBarrasMultidato(string codigoBarras, int userId, int? empresa, out int cantidad)
        {
            var empresas = _context.T_CODIGO_BARRAS
                .Join(_context.T_CODIGO_MULTIDATO_EMPRESA,
                    cb => new { cb.CD_EMPRESA },
                    cm => new { cm.CD_EMPRESA },
                    (cb, cm) => new { Barras = cb, Multidato = cm })
                .Join(_context.T_EMPRESA_FUNCIONARIO,
                    c => new { c.Barras.CD_EMPRESA },
                    ef => new { ef.CD_EMPRESA },
                    (c, ef) => new { Codigo = c, EmpresaFuncionario = ef })
                .Where(x => x.Codigo.Barras.CD_BARRAS == codigoBarras
                    && x.Codigo.Multidato.CD_CODIGO_MULTIDATO == TipoCodigoMultidato.EAN128
                    && x.Codigo.Multidato.FL_HABILITADO == "S"
                    && x.EmpresaFuncionario.USERID == userId
                    && (empresa == null || x.EmpresaFuncionario.CD_EMPRESA == empresa))
                .GroupBy(x => new { x.Codigo.Barras.CD_EMPRESA })
                .OrderBy(x => x.Key.CD_EMPRESA)
                .AsNoTracking()
                .Select(x => x.Key.CD_EMPRESA);

            cantidad = empresas.Count();

            return empresas.FirstOrDefault();
        }
        
        #endregion

        #region Add

        #endregion

        #region Update

        #endregion

        #region Remove

        #endregion

        #region Dapper

        public virtual IEnumerable<CodigoBarras> GetCodigosBarras(IEnumerable<CodigoBarras> codigoBarras)
        {
            IEnumerable<CodigoBarras> resultado = new List<CodigoBarras>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CODIGO_BARRAS_TEMP (CD_BARRAS, CD_EMPRESA) VALUES (:Codigo, :Empresa)";
                    _dapper.Execute(connection, sql, codigoBarras, transaction: tran);

                    sql = GetSqlSelectCodigoBarras() +
                        @"INNER JOIN T_CODIGO_BARRAS_TEMP T ON CB.CD_BARRAS = T.CD_BARRAS AND CB.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<CodigoBarras>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectCodigoBarras()
        {
            return @"SELECT CB.CD_BARRAS AS Codigo
                        , CB.CD_EMPRESA AS Empresa
                        , CB.CD_PRODUTO AS Producto
                        , CB.NU_PRIORIDADE_USO AS PrioridadUso
                        , CB.QT_EMBALAGEM AS CantidadEmbalaje
                        , CB.TP_CODIGO_BARRAS AS TipoCodigo
                        , CB.DT_ADDROW AS FechaInsercion
                        , CB.DT_UPDROW AS FechaModificacion
                        FROM T_CODIGO_BARRAS CB ";
        }

        public virtual async Task<CodigoBarras> GetCodigoBarrasOrNull(string cdBarras, int cdEmpresa, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);
                return GetCodigoBarras(cdBarras, cdEmpresa, connection);
            }
        }

        public virtual CodigoBarras GetCodigoBarras(string cdBarras, int cdEmpresa, System.Data.Common.DbConnection connection)
        {
            string sql = GetSqlSelectCodigoBarras() +
                @"WHERE CB.CD_BARRAS = :Codigo AND CB.CD_EMPRESA = :cdEmpresa";

            return _dapper.Query<CodigoBarras>(connection, sql, param: new
            {
                Codigo = cdBarras,
                cdEmpresa = cdEmpresa
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual async Task AddCodigosBarras(List<CodigoBarras> codigos, ICodigoBarrasServiceContext context, CancellationToken cancelToken = default)
        {
            await AddCodigosBarras(GetBulkOperationContext(codigos, context), cancelToken);
        }

        public virtual async Task AddCodigosBarras(CodigoBarrasBulkOperationContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertProductos(connection, tran, context.NewCodigosBarras);
                    await BulkUpdateProductos(connection, tran, context.UpdCodigosBarras);
                    await BulkDeleteProductos(connection, tran, context.DelCodigosBarras);

                    tran.Commit();
                }
            }
        }

        public virtual CodigoBarrasBulkOperationContext GetBulkOperationContext(List<CodigoBarras> codigosBarras, ICodigoBarrasServiceContext serviceContext)
        {
            var context = new CodigoBarrasBulkOperationContext();
            var newCodigosBarras = new Dictionary<string, CodigoBarras>();

            foreach (var codigoBarras in codigosBarras)
            {
                newCodigosBarras[codigoBarras.Codigo] = codigoBarras;
            }

            foreach (var model in serviceContext.CodigosBarras.Values)
            {
                var codigoBarras = newCodigosBarras[model.Codigo];

                newCodigosBarras.Remove(model.Codigo);
                codigoBarras = Map(codigoBarras, model);

                if (codigoBarras.TipoOperacionId == "B")
                {
                    context.DelCodigosBarras.Add(new
                    {
                        codigo = model.Codigo,
                        empresa = model.Empresa,
                        transaccion = codigoBarras.NumeroTransaccion,
                    });
                }
                else if (codigoBarras.TipoOperacionId == "S")
                {
                    context.UpdCodigosBarras.Add(GetCodigoBarrasEntity(codigoBarras));
                }
            }

            foreach (var codigoBarras in newCodigosBarras.Values)
            {
                context.NewCodigosBarras.Add(GetCodigoBarrasEntity(codigoBarras));
            }

            return context;
        }

        public virtual async Task BulkInsertProductos(DbConnection connection, DbTransaction tran, List<object> codigos)
        {
            string sql = @"INSERT INTO T_CODIGO_BARRAS 
                            (CD_BARRAS, CD_EMPRESA, CD_PRODUTO, TP_CODIGO_BARRAS, NU_PRIORIDADE_USO, QT_EMBALAGEM,  DT_ADDROW, NU_TRANSACCION, NU_TRANSACCION_DELETE) 
                            VALUES(:Codigo, :Empresa, :Producto, :TipoCodigo, :PrioridadUso, :CantidadEmbalaje, :FechaInsercion, :Transaccion, NULL)";

            await _dapper.ExecuteAsync(connection, sql, codigos, transaction: tran);
        }

        public virtual async Task BulkUpdateProductos(DbConnection connection, DbTransaction tran, List<object> codigos)
        {
            string sql = @"
                            UPDATE T_CODIGO_BARRAS SET 
                            CD_PRODUTO = :Producto,
                            TP_CODIGO_BARRAS = :TipoCodigo,
                            NU_PRIORIDADE_USO = :PrioridadUso,
                            QT_EMBALAGEM = :CantidadEmbalaje,
                            DT_UPDROW = :FechaModificacion,
                            NU_TRANSACCION = :Transaccion,
                            NU_TRANSACCION_DELETE = NULL 
                            WHERE CD_BARRAS = :Codigo AND CD_EMPRESA = :Empresa";

            await _dapper.ExecuteAsync(connection, sql, codigos, transaction: tran);
        }

        public static object GetCodigoBarrasEntity(CodigoBarras codigo)
        {
            return new
            {
                Codigo = codigo.Codigo,
                Empresa = codigo.Empresa,
                Producto = codigo.Producto,
                TipoCodigo = codigo.TipoCodigo,
                PrioridadUso = codigo.PrioridadUso,
                CantidadEmbalaje = codigo.CantidadEmbalaje,
                FechaInsercion = codigo.FechaInsercion ?? DateTime.Now,
                FechaModificacion = DateTime.Now,
                Transaccion = codigo.NumeroTransaccion,
            };
        }

        public virtual async Task BulkDeleteProductos(DbConnection connection, DbTransaction tran, List<object> codigos)
        {
            string sql = @" UPDATE T_CODIGO_BARRAS SET 
                NU_TRANSACCION = :transaccion,
                NU_TRANSACCION_DELETE = :transaccion 
                WHERE CD_BARRAS = :codigo AND CD_EMPRESA = :empresa";
            await _dapper.ExecuteAsync(connection, sql, codigos, transaction: tran);

            sql = @" DELETE FROM T_CODIGO_BARRAS WHERE CD_BARRAS = :codigo AND CD_EMPRESA = :empresa";
            await _dapper.ExecuteAsync(connection, sql, codigos, transaction: tran);
        }

        public virtual CodigoBarras Map(CodigoBarras request, CodigoBarras model = null)
        {
            CodigoBarras codigoBarras = new CodigoBarras();

            codigoBarras.CantidadEmbalaje = request.CantidadEmbalaje ?? model?.CantidadEmbalaje;
            codigoBarras.Codigo = request.Codigo;
            codigoBarras.Empresa = request.Empresa;
            codigoBarras.FechaInsercion = request.FechaInsercion ?? model?.FechaInsercion;
            codigoBarras.FechaModificacion = request.FechaModificacion ?? model?.FechaModificacion;
            codigoBarras.PrioridadUso = request.PrioridadUso ?? model?.PrioridadUso;
            codigoBarras.Producto = request.Producto ?? model?.Producto;
            codigoBarras.TipoCodigo = request.TipoCodigo ?? model?.TipoCodigo;
            codigoBarras.TipoOperacionId = request.TipoOperacionId ?? model?.TipoOperacionId;
            codigoBarras.NumeroTransaccion = request.NumeroTransaccion ?? model?.NumeroTransaccion;
            codigoBarras.NumeroTransaccionDelete = request.NumeroTransaccionDelete ?? model?.NumeroTransaccionDelete;

            return codigoBarras;
        }

        #endregion
    }
}
