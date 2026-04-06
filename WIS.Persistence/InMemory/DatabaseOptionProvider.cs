using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace WIS.Persistence.InMemory
{
    public class DatabaseOptionProvider : IDatabaseOptionProvider
    {
        public DbContextOptions<WISDBInMemory> GetDbOptions()
        {
            DbContextOptions<WISDBInMemory> options = new DbContextOptionsBuilder<WISDBInMemory>()
                .UseInMemoryDatabase(databaseName: "WISDBInMemory")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;

            return options;

        }
    }


}
