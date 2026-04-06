using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Eventos
{
    public class EventoTemplate
    {
        public EventoTemplate()
        {
            LstEventInstancias = new List<Instancia>();
        }
        public int nuEvento { get; set; }
        public string CdEstilo { get; set; }

        public string dsEstilo { get; set; }

        public TipoNotificacion TipoNotificacion { get; set; }

        public string Cuerpo { get; set; }

        public string Asunto { get; set; }

        public bool? IsHtml { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? NumeroTransaccion { get; set; }

        public long? NumeroTransaccionDelete { get; set; }

        public List<Instancia> LstEventInstancias { get; set; }
    }
}
