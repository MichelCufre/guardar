using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using WIS.TrackingAPI.Services;

namespace WIS.TrackingAPI.Persistence
{
    public class OracleProvider : IDatabaseProvider
    {
        public DbConnection GetDbConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }

        public string TranslateDapperQuery(string sql)
        {
            return sql;
        }

        public string GetNextSequenceSql(string sequenceName)
        {
            return $"SELECT {sequenceName}.nextval FROM DUAL";
        }
    }
}
