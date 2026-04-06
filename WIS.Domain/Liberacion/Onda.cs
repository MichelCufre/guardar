using System;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Liberacion
{
    public class Onda
    {
        public short Id { get; set; }
        public string Descripcion { get; set; }
        public short? Estado { get; set; }
        public DateTime? FechaSituacion { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Predio { get; set; }



        public virtual void Enable()
        {
            this.Estado = SituacionDb.Activo;
            this.FechaSituacion = DateTime.Now;
        }

        public virtual void Disable()
        {
            this.Estado = SituacionDb.Inactivo;
            this.FechaSituacion = DateTime.Now;
        }

    }
}
