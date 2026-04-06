using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace WIS.Domain.Services.Interfaces
{
    public interface IDatabaseFactory
    {
        void Configure(DbContextOptionsBuilder optionsBuilder);
        DbConnection GetDbConnection();
        string TranslateParameterNames(string sql);
        string TranslateDapperQuery(string sql, CommandType commandType, string overExpression = null);
        string TranslateBulkDapperQuery(string sql, CommandType commandType);
        string GetNextSequenceSql(string sequenceName);
        string GetNextSequencesSql(string sequenceName);
        string GetPageSql(string sql, string orderBy);
        bool IsSnapshotException(Exception ex);
        IsolationLevel GetSnapshotIsolationLevel();
        string GetUpdateSql(string alias, string from, string set, string where);
        string GetDeleteSql(string alias, string from, string where);
        int GetBulkIterationCount(long itemCount, int parametersPerItem, out int itemsPerIteration);
        string GetBlobDataType();
    }
}
