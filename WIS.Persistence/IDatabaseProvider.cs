using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace WIS.Persistence
{
    public interface IDatabaseProvider
    {
        void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString);
        DbConnection GetDbConnection(string connectionString);
        string TranslateParameterNames(string sql);
        string TranslateDapperQuery(string sql, CommandType commandType, string schema, string overExpression = null);
        string TranslateBulkDapperQuery(string sql, CommandType commandType, string schema);
        string GetNextSequenceSql(string sequenceName);
        string GetNextSequencesSql(string sequenceName);
        string GetPageSql(string sql, string orderBy);
        bool IsSnapshotException(Exception ex);
        IsolationLevel GetSnapshotIsolationLevel();
        string GetUpdateSql(string alias, string from, string set, string where);
        string GetDeleteSql(string alias, string from, string where);
        int GetMaxParameterCountPerQuery();
        string GetBlobDataType();
    }
}
