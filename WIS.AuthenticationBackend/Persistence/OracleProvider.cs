using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using WIS.AuthenticationBackend.Services;

namespace WIS.AuthenticationBackend.Persistence
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
    }
}
