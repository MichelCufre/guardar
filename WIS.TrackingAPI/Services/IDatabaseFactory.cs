using System.Data.Common;

namespace WIS.TrackingAPI.Services
{
    public interface IDatabaseFactory
    {
        DbConnection GetDbConnection();
        string TranslateDapperQuery(string sql);
        string GetNextSequenceSql(string sequenceName);
    }
}
