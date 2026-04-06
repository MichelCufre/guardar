using System;

namespace WIS.Domain.Picking
{
    public class Carga
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public short? Ruta { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? Preparacion { get; set; }

        public virtual Carga Copiar()
        {
            return new Carga
            {
                Id = this.Id,
                Descripcion = this.Descripcion,
                Ruta = this.Ruta,
                Preparacion = this.Preparacion
            };
        }
    }
}
