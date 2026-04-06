using System.Data.Common;

namespace WIS.AuthenticationBackend.Services
{
    public interface IDatabaseFactory
    {
        DbConnection GetDbConnection();
        string TranslateDapperQuery(string sql);
    }
}
