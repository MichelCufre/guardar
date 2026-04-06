using System.Collections.Generic;

namespace WIS.Documento.Execution
{
    public class ProduccionDocumentalBlackBoxRequest : ProduccionDocumentalRequest
    {
        public List<LineaReservaDocumentalRequest> Reservas { get; set; }

        public ProduccionDocumentalBlackBoxRequest() : base()
        {
            Reservas = new List<LineaReservaDocumentalRequest>();
        }
    }
}
