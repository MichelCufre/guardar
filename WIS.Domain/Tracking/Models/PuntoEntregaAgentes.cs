using System.Collections.Generic;

namespace WIS.Domain.Tracking.Models
{
    public class PuntoEntregaAgentes
    {
        public string CodigoPuntoEntrega { get; set; }
        public string Zona { get; set; }
        public List<PuntoEntregaAgente> Agentes { get; set; }

        public PuntoEntregaAgentes()
        {
            Agentes = new List<PuntoEntregaAgente>();
        }
    }

    public class PuntoEntregaAgente
    {
        public string Codigo { get; set; }
        public string Tipo { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
