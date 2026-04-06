using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Automatismo.Dtos
{
    public class NotificacionSalidaRequest
    {
        public string Predio { get; set; }
        public string Id { get; set; }
        public short Estado { get; set; }
        public string Usuario { get; set; }
        public string Puesto { get; set; }
        public List<LineaSalida> Detalles { get; set; }
    }

    public class LineaSalida
    {
        public short Id { get; set; }
        public string Producto { get; set; }
        public short Cantidad { get; set; }
        public string Identificador { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Matricula { get; set; }
    }
}
