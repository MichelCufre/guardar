using System.Collections.Generic;

namespace WIS.Domain.Documento.Serializables.Reserva
{
    public class ConsultaAnularReservaPreparacionResponse
    {
        public ConsultaAnularReservaPreparacionResponse()
        {
            this.estadoLineas = new List<EstadoAnulaciones>();
        }

        public bool Succes { get; set; }
        public string MensajeError { get; set; }

        public List<EstadoAnulaciones> estadoLineas { get; set; }
    }

    public class EstadoAnulaciones
    {
        public string IndetificadorAnulacion { get; set; }
        public string Estado { get; set; }
    }
}
