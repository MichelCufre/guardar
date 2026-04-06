using Microsoft.EntityFrameworkCore;

namespace WIS.Persistence
{
    public interface IDatabaseConfigurationService
    {
        void Configure(DbContextOptionsBuilder optionsBuilder);
        string GetBlobDataType();
    }
}
