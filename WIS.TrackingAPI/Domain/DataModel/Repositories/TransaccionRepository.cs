using Dapper;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using WIS.TrackingAPI.Domain.DataModel.Objects;
using WIS.TrackingAPI.Services;

namespace WIS.TrackingAPI.Domain.DataModel.Repositories
{
    public class TransaccionRepository
    {
        private readonly IDapper _dapper;
        private string _application;
        private int _userId;


        public TransaccionRepository(IDapper dapper, string application, int userId)
        {
            this._dapper = dapper; ;
            this._application = application;
            this._userId = userId;
        }

        public async Task<long> CreateTransaction(string dsTransaccion, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                return await CreateTransaction(dsTransaccion, connection, null);
            }
        }

        public async Task<long> CreateTransaction(string dsTransaccion, DbConnection connection, DbTransaction tran)
        {
            return await CreateTransaction(new Transaccion
            {
                CodigoAplicacion = _application,
                DescripcionTransaccion = dsTransaccion,
                CodigoFuncionario = _userId,
            }, connection, tran);
        }

        public async Task<long> CreateTransaction(Transaccion transaccion, DbConnection connection, DbTransaction tran)
        {
            if (transaccion.NumeroTransaccion < 1)
                transaccion.NumeroTransaccion = await GetNextIdTransaccion(connection);

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

        public async Task<long> GetNextIdTransaccion(DbConnection connection)
        {
            return await _dapper.GetNextSequenceValueAsync<long>(connection, "S_TRANSACCION");
        }
    }
}
