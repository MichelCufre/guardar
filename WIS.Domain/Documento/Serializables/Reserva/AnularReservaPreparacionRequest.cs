using System.Collections.Generic;

namespace WIS.Domain.Documento.Serializables.Reserva
{
    public class AnularReservaPreparacionRequest
    {
        public int Usuario { get; set; }
        public string Aplicacion { get; set; }

        public List<AnularReservaPreparacionRequestLinea> LineasAnular { get; set; }
    }
}
