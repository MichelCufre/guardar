using Microsoft.Data.SqlClient;
using System.Data.Common;
using WIS.TrackingAPI.Services;

namespace WIS.TrackingAPI.Persistence
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

        public string GetNextSequenceSql(string sequenceName)
        {
            return $"SELECT NEXT VALUE FOR {sequenceName}";
        }
    }
}
