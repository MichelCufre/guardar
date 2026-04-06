using System.Collections.Generic;

namespace WIS.Domain.Documento.Serializables.Reserva
{
    public class DesreservarStockDocumentalRequest
    {
        public int Usuario { get; set; }
        public string Aplicacion { get; set; }

        public List<DesreservarStockDocumentalRequestLinea> LineasAnular { get; set; }
    }
}
