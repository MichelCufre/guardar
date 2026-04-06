using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WIS.Persistence.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IDapper
    {
        DbConnection GetDbConnection();
        T Get<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text);
        List<T> GetAll<T>(string sql, DynamicParameters parms, CommandType commandType = CommandType.Text);
        int Execute(string sql, object param = null, CommandType commandType = CommandType.Text);
        IEnumerable<T> Query<T>(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null);
        IEnumerable<T> QueryPage<T>(IDbConnection connection, string sql, string orderBy, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null);
        Task<int> ExecuteAsync(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null);
        int Execute(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null);
        Task<T> ExecuteScalarAsync<T>(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null);
        T ExecuteScalar<T>(IDbConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null);
        T GetNextSequenceValue<T>(IDbConnection connection, string sequenceName, IDbTransaction transaction = null);
        IEnumerable<T> GetNextSequenceValues<T>(IDbConnection connection, string sequenceName, int count, IDbTransaction transaction = null);
        Task<T> GetNextSequenceValueAsync<T>(IDbConnection connection, string sequenceName, IDbTransaction transaction = null);
        int ExecuteUpdate(IDbConnection connection, string alias, string from, string set, string where, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null);
        int ExecuteDelete(IDbConnection connection, string alias, string from, string where, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null);
        Task<int> ExecuteUpdateAsync(IDbConnection connection, string alias, string from, string set, string where, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null);
        IsolationLevel GetSnapshotIsolationLevel();
        void BulkInsert<T>(IDbConnection connection, IDbTransaction transaction, IEnumerable<T> items, string tableName, Dictionary<string, Func<T, ColumnInfo>> columnSelectors) where T : class;
        void BulkUpdate<T>(IDbConnection connection, IDbTransaction transaction, IEnumerable<T> items, string tableName, Dictionary<string, Func<T, ColumnInfo>> columnSelectors, Dictionary<string, Func<T, ColumnInfo>> keySelector) where T : class;
        void BulkDelete<T>(IDbConnection connection, IDbTransaction transaction, IEnumerable<T> items, string tableName, Dictionary<string, Func<T, object>> keySelector) where T : class;
        
    }
}
