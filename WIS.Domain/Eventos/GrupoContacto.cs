using System;
using System.Collections.Generic;

namespace WIS.Domain.Eventos
{
    public partial class GrupoContacto 
    {
        public List<Contacto> Contactos { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? NumeroTransaccion { get; set; }

        public long? NumeroTransaccionDelete { get; set; }

        public GrupoContacto()
        {
            Contactos = new List<Contacto>();
        }
    }
}
