using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WIS.AuthenticationBackend.DataContext
{
    public class AppContext : DbContext
    {
        public AppContext() { }
        public AppContext(DbContextOptions<AppContext> options) : base(options) { }
    }
}
