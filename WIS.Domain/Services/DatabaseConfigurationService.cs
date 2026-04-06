using Microsoft.EntityFrameworkCore;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;

namespace WIS.Domain.Services
{
    public class DatabaseConfigurationService : IDatabaseConfigurationService
    {
        protected readonly IDatabaseFactory _dbFactory;

        public DatabaseConfigurationService(IDatabaseFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public virtual void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            _dbFactory.Configure(optionsBuilder);
        }

        public virtual string GetBlobDataType()
        {
            return _dbFactory.GetBlobDataType();
        }

    }
}