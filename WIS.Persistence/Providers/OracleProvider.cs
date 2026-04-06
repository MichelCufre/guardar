using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.Common;

namespace WIS.Persistence.Providers
{
    public class OracleProvider : IDatabaseProvider
    {
        public void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.UseOracle(connectionString);
        }

        public DbConnection GetDbConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }

        public string TranslateParameterNames(string sql)
        {
            return sql;
        }

        public string TranslateDapperQuery(string sql, CommandType commandType, string schema, string overExpression = null)
        {
            return sql.Replace("WISFUN!", "");
        }

        public string TranslateBulkDapperQuery(string sql, CommandType commandType, string schema)
        {
            return $"BEGIN {TranslateDapperQuery(sql, commandType, schema)} END;";
        }

        public string GetNextSequenceSql(string sequenceName)
        {
            return $"SELECT {sequenceName}.nextval FROM DUAL";
        }

        public string GetNextSequencesSql(string sequenceName)
        {
            return $@"
                SELECT {sequenceName}.nextval 
                FROM (
                    SELECT level 
                    FROM DUAL 
                    CONNECT BY level <= :count
                )";
        }
        public bool IsSnapshotException(Exception ex)
        {
            return ex is Oracle.ManagedDataAccess.Client.OracleException
                && ex.Message.Contains("ORA-08177:");
        }
        public string GetPageSql(string sql, string orderBy)
        {
            return @$"
                SELECT * 
                FROM ( 
                    SELECT 
                        a.*, 
                        rownum nuRow 
                    FROM ({sql}{orderBy}) a 
                    WHERE rownum < ((:pageNumber * :pageSize) + 1 ) 
                ) 
                WHERE nuRow >= (((:pageNumber-1) * :pageSize) + 1)";
        }

        public string GetUpdateSql(string alias, string from, string set, string where)
        {
            return $@"
                UPDATE (SELECT * FROM {from})
                SET {set}
                {(string.IsNullOrEmpty(where) ? "" : ($"WHERE {where}"))}
            ";
        }
        public string GetDeleteSql(string alias, string from, string where)
        {
            return $@"
                DELETE (SELECT * FROM {from})
                {(string.IsNullOrEmpty(where) ? "" : ($"WHERE {where}"))}
            ";
        }
        public IsolationLevel GetSnapshotIsolationLevel()
        {
            return IsolationLevel.Serializable;
        }

        public int GetMaxParameterCountPerQuery()
        {
            return 32766;
        }

        public string GetBlobDataType()
        {
            return "BLOB";
        }
    }
}
