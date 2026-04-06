using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WIS.Persistence.Database;

namespace WIS.XmlData.WInterface.Helpers
{
    public class TransactionHelper
    {
        public virtual IDbContextTransaction BeginTransaction(WISDB context)
        {
            return context.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }
    }
}
