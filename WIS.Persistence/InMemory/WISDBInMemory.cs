using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WIS.Persistence.InMemory
{
    public class WISDBInMemory : DbContext
    {
        private static long _secNumberTransaction = 1;
        private long _numTran = 0;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PtlColorEnUsoEntity>()
                .HasKey(c => new { c.NU_PTL, c.NU_COLOR });
            modelBuilder.Entity<PtlPosicionEnUsoEntity>()
                .HasKey(c => new { c.NU_ADDRESS, c.NU_COLOR, c.NU_ORDEN });
        }

        public WISDBInMemory(IDatabaseOptionProvider options) : base(options.GetDbOptions())
        {
        }

        public DbSet<PtlColorEnUsoEntity> PtlColorEnUsoEntity { get; set; }
        public DbSet<PtlPosicionEnUsoEntity> PtlPosicionEnUsoEntity { get; set; }

        public void CreateNumberTransaccion()
        {
            this._numTran = _secNumberTransaction++;
        }

        public long GetNumberTransaction()
        {
            return this._numTran;
        }

        public void RemoveAllByTransaction()
        {
            var colores = this.PtlColorEnUsoEntity.Where(w => w.Transaccion == this._numTran);

            if (colores != null)
                this.PtlColorEnUsoEntity.RemoveRange(colores);

            var posiciones = this.PtlPosicionEnUsoEntity.Where(w => w.Transaccion == this._numTran);

            if (posiciones != null)
                this.PtlPosicionEnUsoEntity.RemoveRange(posiciones);
        }

        public void ClearTransaction()
        {
            this._numTran = 0;
        }
    }
}
