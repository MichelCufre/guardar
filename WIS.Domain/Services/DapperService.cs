using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.General;

namespace WIS.Domain.Services
{
    public class DapperService : IDapper
    {
        protected readonly IDatabaseFactory _dbFactory;
        protected DbConnection _conn;
        protected int _internalTimeout;

        public DapperService(
            IDatabaseFactory dbFactory,
            IOptions<ApplicationSettings> appSettings)
        {
            _dbFactory = dbFactory;
            _internalTimeout = (appSettings.Value.InternalTimeout ?? 30) * 60;
            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
        }

        public DapperService(IDatabaseFactory dbFactory, DbConnection conn)
        {
            _dbFactory = dbFactory;
            _conn = conn;
            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
        }

        public virtual void Dispose()
        {
            _conn.Close();
        }

        public virtual DbConnection GetDbConnection()
        {
            _conn = _dbFactory.GetDbConnection();
            return _conn;
        }

        public virtual T Get<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using (IDbConnection db = GetDbConnection())
            {
                db.Open();
                sql = _dbFactory.TranslateDapperQuery(sql, commandType);
                return db.Query<T>(sql, parms, commandType: commandType, commandTimeout: _internalTimeout).FirstOrDefault();
            }
        }

        public virtual List<T> GetAll<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using (IDbConnection db = GetDbConnection())
            {
                db.Open();
                sql = _dbFactory.TranslateDapperQuery(sql, commandType);
                return db.Query<T>(sql, parms, commandType: commandType, commandTimeout: _internalTimeout).ToList();
            }
        }

        public virtual int Execute(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            int result;

            using (IDbConnection db = GetDbConnection())
            {
                db.Open();

                using (var tran = db.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    sql = _dbFactory.TranslateDapperQuery(sql, commandType);
                    result = db.Execute(sql, param: param, commandType: commandType, commandTimeout: _internalTimeout, transaction: tran);
                    tran.Commit();
                }
            }

            return result;
        }

        public virtual IEnumerable<T> Query<T>(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        {
            sql = _dbFactory.TranslateDapperQuery(sql, commandType);
            return connection.Query<T>(sql, param: param, commandType: commandType, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual IEnumerable<T> QueryPage<T>(IDbConnection connection, string sql, string orderBy, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        {
            sql = _dbFactory.GetPageSql(sql, orderBy);
            sql = _dbFactory.TranslateDapperQuery(sql, commandType);
            return connection.Query<T>(sql, param: param, commandType: commandType, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual async Task<int> ExecuteAsync(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        {
            sql = _dbFactory.TranslateDapperQuery(sql, commandType);
            return await connection.ExecuteAsync(sql, param: param, commandType: commandType, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual int Execute(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        {
            sql = _dbFactory.TranslateDapperQuery(sql, commandType);
            return connection.Execute(sql, param: param, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual async Task<T> ExecuteScalarAsync<T>(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        {
            sql = _dbFactory.TranslateDapperQuery(sql, commandType);
            return await connection.ExecuteScalarAsync<T>(sql, param: param, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual T ExecuteScalar<T>(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        {
            sql = _dbFactory.TranslateDapperQuery(sql, commandType);
            return connection.ExecuteScalar<T>(sql, param: param, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual T GetNextSequenceValue<T>(IDbConnection connection, string sequenceName, IDbTransaction transaction = null)
        {
            var sql = _dbFactory.GetNextSequenceSql(sequenceName);
            return connection.ExecuteScalar<T>(sql, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual IEnumerable<T> GetNextSequenceValues<T>(IDbConnection connection, string sequenceName, int count, IDbTransaction transaction = null)
        {
            var sql = _dbFactory.GetNextSequencesSql(sequenceName);
            return connection.Query<T>(sql, param: new { count = count }, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual async Task<T> GetNextSequenceValueAsync<T>(IDbConnection connection, string sequenceName, IDbTransaction transaction = null)
        {
            var sql = _dbFactory.GetNextSequenceSql(sequenceName);
            return await connection.ExecuteScalarAsync<T>(sql, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual int ExecuteUpdate(IDbConnection connection, string alias, string from, string set, string where, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        {
            var sql = _dbFactory.GetUpdateSql(alias, from, set, where);
            sql = _dbFactory.TranslateDapperQuery(sql, commandType);
            return connection.Execute(sql, param: param, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual int ExecuteDelete(IDbConnection connection, string alias, string from, string where, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        {
            var sql = _dbFactory.GetDeleteSql(alias, from,  where);
            sql = _dbFactory.TranslateDapperQuery(sql, commandType);
            return connection.Execute(sql, param: param, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual async Task<int> ExecuteUpdateAsync(IDbConnection connection, string alias, string from, string set, string where, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        {
            var sql = _dbFactory.GetUpdateSql(alias, from, set, where);
            sql = _dbFactory.TranslateDapperQuery(sql, commandType);
            return await connection.ExecuteAsync(sql, param: param, commandTimeout: _internalTimeout, transaction: transaction);
        }

        public virtual IsolationLevel GetSnapshotIsolationLevel()
        {
            return _dbFactory.GetSnapshotIsolationLevel();
        }

        public virtual void BulkInsert<T>(IDbConnection connection, IDbTransaction transaction, IEnumerable<T> items, string tableName, Dictionary<string, Func<T, ColumnInfo>> columnSelectors) where T : class
        {
            if (items.Count() > 0)
            {
                var columns = columnSelectors.Keys.Order();
                var iterationCount = _dbFactory.GetBulkIterationCount(items.Count(), columnSelectors.Count(), out int itemsPerIteration);
                for (int i = 0; i < iterationCount; i++)
                {
                    var firstItem = i * itemsPerIteration;
                    var lastItem = Math.Min(firstItem + itemsPerIteration, items.Count()) - 1;

                    if (firstItem <= lastItem)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            for (int j = firstItem; j <= lastItem; j++)
                            {
                                var item = items.ElementAt(j);
                                var parameters = new List<string>();
                                var columnsExcept = new List<string>();
                                foreach (var columnName in columns)
                                {
                                    var parameter = command.CreateParameter();
                                    var columnInfo = columnSelectors[columnName](item);

                                    parameter.ParameterName = _dbFactory.TranslateParameterNames($":P_{j}_{columnName}");
                                    parameter.Value = columnInfo.Value ?? DBNull.Value;
                                    
                                    if (parameter.Value is string)
                                        parameter.DbType = DbType.AnsiString;
                                    else
                                        parameter.DbType = columnInfo.Type;

                                    command.Parameters.Add(parameter);
                                    parameters.Add(parameter.ParameterName);
                                }

                                command.CommandText += $"INSERT INTO {tableName} ({string.Join(",", columns.Except(columnsExcept))}) VALUES ({string.Join(",", parameters)});";
                            }

                            command.Transaction = transaction;
                            command.CommandTimeout = _internalTimeout;
                            command.CommandText = _dbFactory.TranslateBulkDapperQuery(command.CommandText, CommandType.Text);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public virtual void BulkUpdate<T>(IDbConnection connection, IDbTransaction transaction, IEnumerable<T> items, string tableName, Dictionary<string, Func<T, ColumnInfo>> columnSelectors, Dictionary<string, Func<T, ColumnInfo>> keySelector) where T : class
        {
            if (items.Count() > 0)
            {
                var columns = columnSelectors.Keys.Order();
                var iterationCount = _dbFactory.GetBulkIterationCount(items.Count(), columnSelectors.Count() + keySelector.Count(), out int itemsPerIteration);

                for (int i = 0; i < iterationCount; i++)
                {
                    var firstItem = i * itemsPerIteration;
                    var lastItem = Math.Min(firstItem + itemsPerIteration, items.Count()) - 1;

                    if (firstItem <= lastItem)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            for (int j = firstItem; j <= lastItem; j++)
                            {
                                var item = items.ElementAt(j);
                                var setClauses = new List<string>();
                                var keyConditions = new List<string>();

                                foreach (var columnName in columns)
                                {
                                    var parameter = command.CreateParameter();
                                    var columnInfo = columnSelectors[columnName](item);

                                    parameter.ParameterName = _dbFactory.TranslateParameterNames($":P_{j}_{columnName}");
                                    parameter.Value = columnInfo.Value ?? DBNull.Value;
                                    
                                    if (parameter.Value is string)
                                        parameter.DbType = DbType.AnsiString;
                                    else
                                        parameter.DbType = columnInfo.Type;

                                    command.Parameters.Add(parameter);

                                    if (OperacionDb.OperacionMas == columnInfo.Operacion || OperacionDb.OperacionMenos == columnInfo.Operacion)
                                        setClauses.Add($"{columnName} = {columnName} {columnInfo.Operacion} {parameter.ParameterName}");
                                    else
                                        setClauses.Add($"{columnName} = {parameter.ParameterName}");
                                }

                                foreach (var columnName in keySelector.Keys)
                                {
                                    var parameter = command.CreateParameter();
                                    var columnInfo = keySelector[columnName](item);

                                    parameter.ParameterName = _dbFactory.TranslateParameterNames($":K_{j}_{columnName}");
                                    parameter.Value = columnInfo.Value ?? DBNull.Value;

                                    if (parameter.Value is string)
                                        parameter.DbType = DbType.AnsiString;
                                    else
                                        parameter.DbType = columnInfo.Type;

                                    command.Parameters.Add(parameter);
                                    keyConditions.Add($"{columnName} = {parameter.ParameterName}");
                                }

                                command.CommandText += $"UPDATE {tableName} SET {string.Join(",", setClauses)} WHERE {string.Join(" AND ", keyConditions)};";
                            }

                            command.Transaction = transaction;
                            command.CommandTimeout = _internalTimeout;
                            command.CommandText = _dbFactory.TranslateBulkDapperQuery(command.CommandText, CommandType.Text);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public virtual void BulkDelete<T>(IDbConnection connection, IDbTransaction transaction, IEnumerable<T> items, string tableName, Dictionary<string, Func<T, object>> keySelector) where T : class
        {
            if (items.Count() > 0)
            {
                var iterationCount = _dbFactory.GetBulkIterationCount(items.Count(), keySelector.Count(), out int itemsPerIteration);

                for (int i = 0; i < iterationCount; i++)
                {
                    var firstItem = i * itemsPerIteration;
                    var lastItem = Math.Min(firstItem + itemsPerIteration, items.Count()) - 1;

                    if (firstItem <= lastItem)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            for (int j = firstItem; j <= lastItem; j++)
                            {
                                var item = items.ElementAt(j);
                                var keyConditions = new List<string>();

                                foreach (var columnName in keySelector.Keys)
                                {
                                    var parameter = command.CreateParameter();

                                    parameter.ParameterName = _dbFactory.TranslateParameterNames($":K_{j}_{columnName}");
                                    parameter.Value = keySelector[columnName](item);

                                    if (parameter.Value is string)
                                        parameter.DbType = DbType.AnsiString;

                                    command.Parameters.Add(parameter);
                                    keyConditions.Add($"{columnName} = {parameter.ParameterName}");
                                }

                                command.CommandText += $"DELETE FROM {tableName} WHERE {string.Join(" AND ", keyConditions)};";
                            }

                            command.Transaction = transaction;
                            command.CommandTimeout = _internalTimeout;
                            command.CommandText = _dbFactory.TranslateBulkDapperQuery(command.CommandText, CommandType.Text);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

    }
}
