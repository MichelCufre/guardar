using Dapper;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TransaccionRepository
    {
        protected WISDB _context;
        protected string _application;
        protected int _userId;
        protected TransaccionMapper _mapper;
        protected readonly IDapper _dapper;

        public TransaccionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new TransaccionMapper();
            this._dapper = dapper;
        }

        public virtual long AddTransaccion(string descripcion)
        {
            return AddTransaccion(descripcion, _application, _userId);
        }

        public virtual long AddTransaccion(string descripcion, string application, int userId)
        {
            long nutran = _context.GetNextSequenceValueLong(_dapper, "S_TRANSACCION");

            T_TRANSACCION entity = this._mapper.MapToEntity(new Transaccion
            {
                NumeroTransaccion = nutran,
                CodigoAplicacion = application,
                CodigoFuncionario = userId,
                FechaAlta = DateTime.Now,
                DescripcionTransaccion = descripcion,
            });

            this._context.T_TRANSACCION.Add(entity);

            return nutran;
        }

        #region Dapper

        public virtual async Task<long> CreateTransaction(string descripcion, DbConnection connection, DbTransaction tran = null, string app = null, int? userId = null)
        {
            return await CreateTransaction(new Transaccion
            {
                DescripcionTransaccion = descripcion,
                CodigoAplicacion = !string.IsNullOrEmpty(app) ? (app.Length > 30 ? app.Substring(0, 30) : app) : (_application.Length > 30 ? _application.Substring(0, 30) : _application),
                CodigoFuncionario = userId ?? _userId,
            }, connection, tran);
        }

        public virtual async Task<long> CreateTransaction(Transaccion transaccion, DbConnection connection, DbTransaction tran)
        {
            if (transaccion.NumeroTransaccion < 1)
                transaccion.NumeroTransaccion = await GetNextIdTransaccion(connection, tran);

            var param = new DynamicParameters(new
            {
                FechaAlta = DateTime.Now,
                NumeroTransaccion = transaccion.NumeroTransaccion,
                CodigoAplicacion = transaccion.CodigoAplicacion,
                DescripcionTransaccion = transaccion.DescripcionTransaccion,
                CodigoFuncionario = transaccion.CodigoFuncionario,
                Data = transaccion.Data
            });

            string sql = @"INSERT INTO T_TRANSACCION (NU_TRANSACCION, DS_TRANSACCION, CD_APLICACION, CD_FUNCIONARIO, VL_DATA, DT_ADDROW) 
                         VALUES (:NumeroTransaccion, :DescripcionTransaccion, :CodigoAplicacion, :CodigoFuncionario, :Data, :FechaAlta)";
            await _dapper.ExecuteAsync(connection, sql, param: param, transaction: tran);

            return transaccion.NumeroTransaccion;
        }

        public virtual async Task<long> GetNextIdTransaccion(DbConnection connection, IDbTransaction transaction = null)
        {
            return await _dapper.GetNextSequenceValueAsync<long>(connection, "S_TRANSACCION", transaction);
        }

        public virtual void SetAplicacion(string app, DbConnection connection, IDbTransaction transaction = null)
        {
            string sql = "K_USUARIO.SET_APLICACAO";
            _dapper.Query<object>(connection, sql, new
            {
                P_APLIC = app
            }, commandType: CommandType.StoredProcedure, transaction);
        }

        public virtual void SetTransaccion(long nuTransaccion, DbConnection connection, IDbTransaction transaction = null)
        {
            string sql = "K_USUARIO.SET_TRANSACCION";
            _dapper.Query<object>(connection, sql, new
            {
                P_TRANS = nuTransaccion
            }, commandType: CommandType.StoredProcedure, transaction);
        }

        public virtual void SetFuncionario(int userId, DbConnection connection, IDbTransaction transaction = null)
        {
            string sql = "K_USUARIO.SET_FUNCIONARIO";
            _dapper.Query<object>(connection, sql, new
            {
                P_FUNC = userId
            }, commandType: CommandType.StoredProcedure, transaction);
        }
        #endregion
    }
}
