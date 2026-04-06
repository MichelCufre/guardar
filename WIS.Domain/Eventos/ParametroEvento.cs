using System.Collections.Generic;

namespace WIS.Domain.Eventos
{

    public partial class ParametroEvento
    {
        public ParametroEvento()
        {
            ParametrosInstancias = new List<ParametroInstancia>();
        }

        public string Codigo { get; set; }

        public int NumeroEvento { get; set; }

        public string Descripcion { get; set; }

        public bool EsRequerido { get; set; }

        public TipoNotificacion TipoNotificacion { get; set; }

        public string ExpresionRegular { get; set; }

        public Evento Evento { get; set; }

        public List<ParametroInstancia> ParametrosInstancias { get; set; }
    }
}
