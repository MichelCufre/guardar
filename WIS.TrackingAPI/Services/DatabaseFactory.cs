using Microsoft.Extensions.Configuration;
using System.Data.Common;
using WIS.TrackingAPI.Persistence;

namespace WIS.TrackingAPI.Services
{
    public class DatabaseFactory : IDatabaseFactory
    {
        protected readonly IConfiguration _config;

        public DatabaseFactory(IConfiguration config)
        {
            _config = config;
        }

        protected IDatabaseProvider Provider
        {
            get
            {
                var provider = _config.GetSection("DatabaseSettings:Provider").Value ?? "Oracle";

                switch (provider)
                {
                    case "SqlServer":
                        return new SqlServerProvider();
                    default:
                        return new OracleProvider();
                }
            }
        }

        public DbConnection GetDbConnection()
        {
            var connectionString = _config.GetConnectionString("WISDB");
            return Provider.GetDbConnection(connectionString);
        }

        public string TranslateDapperQuery(string sql)
        {
            return Provider.TranslateDapperQuery(sql);
        }

        public string GetNextSequenceSql(string sequenceName)
        {
            return Provider.GetNextSequenceSql(sequenceName);
        }
    }
}
