using Custom.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Persistence;
using WIS.Persistence.Database;

namespace Custom.Persistence.Database
{
    public partial class CUSTOMDB : WISDB
    {
        public CUSTOMDB(IDatabaseConfigurationService dbConfigService, string connectionString, string schema) 
            :base(dbConfigService, connectionString, schema)
        {

        }

        #region Tablas
        #endregion

        #region Vistas

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Claves compuestas

            #region Tablas
            #endregion

            #region Vistas

            #endregion

            #endregion

            #region Relaciones

            #endregion

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()).Where(p => p.ClrType == typeof(string) && (p.IsUnicode() ?? true) && p.GetColumnType() == null))
            {
                property.SetIsUnicode(false);
            }
        }
    }
}