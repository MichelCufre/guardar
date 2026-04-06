using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.Common;
using WIS.Configuration;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.Persistence.Providers;

namespace WIS.Domain.Services
{
    public class DatabaseFactory : IDatabaseFactory
    {
        protected readonly IOptions<DatabaseSettings> _config;

        public DatabaseFactory(IOptions<DatabaseSettings> config)
        {
            _config = config;
        }

        protected IDatabaseProvider Provider
        {
            get
            {
                Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);

                var provider = _config.Value.Provider ?? "Oracle";

                switch (provider)
                {
                    case "SqlServer":
                        return new SqlServerProvider();
                    default:
                        return new OracleProvider();
                }
            }
        }

        public virtual void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _config.Value.ConnectionString;

            Provider.Configure(optionsBuilder, connectionString);

            optionsBuilder.UseUpperCaseNamingConvention();
        }

        public virtual DbConnection GetDbConnection()
        {
            var connectionString = _config.Value.ConnectionString;
            return Provider.GetDbConnection(connectionString);
        }

        public virtual string TranslateParameterNames(string sql)
        {
            return Provider.TranslateParameterNames(sql);
        }

        public virtual string TranslateDapperQuery(string sql, CommandType commandType, string overExpression = null)
        {
            var schema = _config.Value.Schema;
            return Provider.TranslateDapperQuery(sql, commandType, schema, overExpression);
        }

        public virtual string TranslateBulkDapperQuery(string sql, CommandType commandType)
        {
            var schema = _config.Value.Schema;
            return Provider.TranslateBulkDapperQuery(sql, commandType, schema);
        }

        public virtual string GetNextSequenceSql(string sequenceName)
        {
            return Provider.GetNextSequenceSql(sequenceName);
        }

        public virtual string GetNextSequencesSql(string sequenceName)
        {
            return Provider.GetNextSequencesSql(sequenceName);
        }
        public virtual string GetPageSql(string sql, string orderBy)
        {
            return Provider.GetPageSql(sql, orderBy);
        }

        public virtual bool IsSnapshotException(Exception ex)
        {
            return Provider.IsSnapshotException(ex);
        }

        public virtual IsolationLevel GetSnapshotIsolationLevel()
        {
            return Provider.GetSnapshotIsolationLevel();
        }

        public virtual string GetUpdateSql(string alias, string from, string set, string where)
        {
            return Provider.GetUpdateSql(alias, from, set, where);
        }
        public virtual string GetDeleteSql(string alias, string from, string where)
        {
            return Provider.GetDeleteSql(alias, from, where);
        }
        public virtual int GetBulkIterationCount(long itemCount, int parametersPerItem, out int itemsPerIteration)
        {
            var maxParameterCount = _config.Value.MaxParameterCountPerQuery ?? (decimal)Provider.GetMaxParameterCountPerQuery();

            itemsPerIteration = (int)Math.Floor(maxParameterCount / (decimal)parametersPerItem);

            return (int)Math.Ceiling((decimal)itemCount / (decimal)itemsPerIteration);
        }

        public virtual string GetBlobDataType()
        {
            return Provider.GetBlobDataType();
        }
    }
}
