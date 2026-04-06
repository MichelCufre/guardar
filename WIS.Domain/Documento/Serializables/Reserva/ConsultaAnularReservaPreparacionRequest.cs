using System.Collections.Generic;

namespace WIS.Domain.Documento.Serializables.Reserva
{
    public class ConsultaAnularReservaPreparacionRequest
    {
        public int Usuario { get; set; }
        public string Aplicacion { get; set; }
        public List<string> IdentificadoresAnulacion { get; set; }
    }
}
