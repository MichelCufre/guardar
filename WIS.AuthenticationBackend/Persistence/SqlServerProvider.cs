using Microsoft.Data.SqlClient;
using System.Data.Common;
using WIS.AuthenticationBackend.Services;

namespace WIS.AuthenticationBackend.Persistence
{
    public class SqlServerProvider : IDatabaseProvider
    {
        public DbConnection GetDbConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public string TranslateDapperQuery(string sql)
        {
            sql = sql.Replace("||", "+");
            return sql.Replace(":", "@");
        }
    }
}
