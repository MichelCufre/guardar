using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Eventos
{
    public class ContactoGrupo
    {
        public int NumeroContactoGrupo { get; set; }

        public int NumeroContacto { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? NumeroTransaccion { get; set; }

        public long? NumeroTransaccionDelete { get; set; }

    }
}
