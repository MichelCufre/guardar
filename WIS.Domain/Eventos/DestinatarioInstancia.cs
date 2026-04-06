using System;
using System.Collections.Generic;

namespace WIS.Domain.Eventos
{
    public partial class DestinatarioInstancia 
    {
        public int Id { get; set; }

        public int NumeroInstancia { get; set; }

        public int? NumeroGrupo { get; set; }

        public int? NumeroContacto { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? NumeroTransaccion { get; set; }

        public long? NumeroTransaccionDelete { get; set; }
    }
}
