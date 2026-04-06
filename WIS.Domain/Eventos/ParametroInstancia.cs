using System;

namespace WIS.Domain.Eventos
{
    public partial class ParametroInstancia
    {
        public string Codigo { get; set; }

        public int NumeroInstancia { get; set; }

        public string Valor { get; set; }
        public TipoNotificacion TipoNotificacion { get; set; }

        public Instancia Instancia { get; set; }

        public ParametroEvento ParametroEvento { get; set; }

        public int NuEvento { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public long? NumeroTransaccion { get; set; }

        public long? NumeroTransaccionDelete { get; set; }

    }
}
