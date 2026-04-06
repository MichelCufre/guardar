using Microsoft.EntityFrameworkCore;

namespace WIS.Persistence.InMemory
{
    public interface IDatabaseOptionProvider
    {
        DbContextOptions<WISDBInMemory> GetDbOptions();
    }
}
