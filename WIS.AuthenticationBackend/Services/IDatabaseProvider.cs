using System.Data.Common;

namespace WIS.AuthenticationBackend.Services
{
    public interface IDatabaseProvider
    {
        DbConnection GetDbConnection(string connectionString);
        string TranslateDapperQuery(string sql);
    }
}
