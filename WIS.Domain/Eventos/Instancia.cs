using System;
using System.Collections.Generic;

namespace WIS.Domain.Eventos
{
    public partial class Instancia
    {
        public Instancia()
        {
            Notificaciones = new List<Notificacion>();
            Parametros = new List<ParametroInstancia>();
            Template = new EventoTemplate();
        }

        public int Id { get; set; }

        public int NumeroEvento { get; set; }

        public string NombreEvento { get; set; }

        public string Descripcion { get; set; }

        public bool EsHabilitado { get; set; }

        public string Plantilla { get; set; }

        public TipoNotificacion TipoNotificacion { get; set; }

        public Evento Evento { get; set; }

        public List<Notificacion> Notificaciones { get; set; }

        public List<ParametroInstancia> Parametros { get; set; }

        public EventoTemplate Template { get; set; }

        public string IdTipoNotificacion { get; set; }
        
        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? NumeroTransaccion { get; set; }

        public long? NumeroTransaccionDelete { get; set; }
    }
}
