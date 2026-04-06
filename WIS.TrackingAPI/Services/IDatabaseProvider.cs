using System.Data.Common;

namespace WIS.TrackingAPI.Services
{
    public interface IDatabaseProvider
    {
        DbConnection GetDbConnection(string connectionString);
        string TranslateDapperQuery(string sql);
        string GetNextSequenceSql(string sequenceName);
    }
}
