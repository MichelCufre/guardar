using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using WIS.Persistence.Interceptors;

namespace WIS.Persistence.Providers
{
    public class SqlServerProvider : IDatabaseProvider
    {
        public void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.UseSqlServer(connectionString).AddInterceptors(new VarcharInterceptor());
        }

        public DbConnection GetDbConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public string TranslateParameterNames(string sql)
        {
            return sql.Replace(":", "@");
        }

        public string TranslateDapperQuery(string sql, CommandType commandType, string schema, string overExpression = null)
        {
            if (commandType == CommandType.StoredProcedure)
                return sql.Replace(".", "$");
            else
            {
                sql = sql.Replace("MOD(", $"{schema}.MOD(");
                sql = sql.Replace("TO_CHAR(", $"{schema}.TO_CHAR(");
                sql = sql.Replace("DATEADD('DAY',", $"DATEADD(DAY,");
                sql = sql.Replace("DATEADD('MONTH',", $"DATEADD(MONTH,");
                sql = sql.Replace("DATEADD('YEAR',", $"DATEADD(YEAR,");
                sql = sql.Replace("DATEADD('HOUR',", $"DATEADD(HOUR,");
                sql = sql.Replace("||", "+");
                sql = sql.Replace("ROWNUM", $"ROW_NUMBER() OVER({overExpression})");
                sql = TranslateParameterNames(sql);
                sql = TranslateWISFunctionCalls(sql, schema);

                return sql;
            }
        }

        public string TranslateBulkDapperQuery(string sql, CommandType commandType, string schema)
        {
            return TranslateDapperQuery(sql, commandType, schema);
        }

        protected string TranslateWISFunctionCalls(string sql, string schema)
        {
            var startIndex = sql.IndexOf("WISFUN!", 0);
            var names = new HashSet<string>();

            while (startIndex >= 0)
            {
                var endIndex = sql.IndexOf("(", startIndex);
                var name = sql.Substring(startIndex, endIndex - startIndex);

                if (!names.Contains(name))
                    names.Add(name);

                startIndex = sql.IndexOf("WISFUN!", endIndex);
            }

            foreach (var name in names)
            {
                sql = sql.Replace(name, name.Replace(".", "$").Replace("WISFUN!", $"{schema}."));
            }

            return sql;
        }

        public string GetNextSequenceSql(string sequenceName)
        {
            return $"SELECT NEXT VALUE FOR {sequenceName}";
        }

        public string GetNextSequencesSql(string sequenceName)
        {
            return $@"SELECT NEXT VALUE FOR {sequenceName} FROM GENERATE_SERIES(1, @count) WHERE @count > 0;";
        }

        public bool IsSnapshotException(Exception ex)
        {
            return ex is SqlException && (ex as SqlException).Number == 3960;
        }
        public string GetPageSql(string sql, string orderBy)
        {
            orderBy = orderBy.Replace("ORDER BY ", "", StringComparison.InvariantCultureIgnoreCase);

            var orderByCols = orderBy.Split(',');

            for (int i = 0; i < orderByCols.Length; i++)
            {
                orderByCols[i] = $"a.{orderByCols[i]}";
            }

            orderBy = $"ORDER BY {string.Join(",", orderByCols)}";

            return @$"
                SELECT b.* 
                FROM ( 
                    SELECT 
                        a.*, 
                        ROW_NUMBER() OVER({orderBy}) AS nuRow 
                    FROM ({sql}) a 
                ) b
                WHERE b.nuRow < ((:pageNumber * :pageSize) + 1 ) 
                    AND b.nuRow >= (((:pageNumber-1) * :pageSize) + 1)";
        }

        public string GetUpdateSql(string alias, string from, string set, string where)
        {
            return $@"
                UPDATE {alias}
                SET {set}
                FROM {from}
                {(string.IsNullOrEmpty(where) ? "" : ($"WHERE {where}"))}
            ";
        }
        public string GetDeleteSql(string alias, string from, string where)
        {
            return $@"
                DELETE {alias}
                FROM {from}
                {(string.IsNullOrEmpty(where) ? "" : ($"WHERE {where}"))}
            ";
        }
        public IsolationLevel GetSnapshotIsolationLevel()
        {
            return IsolationLevel.Snapshot;
        }

        public int GetMaxParameterCountPerQuery()
        {
            return 2099;
        }

        public string GetBlobDataType()
        {
            return null;
        }
    }
}
