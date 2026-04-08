using System;

namespace Custom.Persistence.Entities
{
    public class MiddlewareCola
    {
        public long Id { get; set; }
        public string Tipo { get; set; }       // PRODUCTO | AGENTE | CODIGOBARRAS | PEDIDO
        public string Payload { get; set; }    // JSON serializado del WmsRequest
        public string Estado { get; set; }     // PENDIENTE | PROCESADO | ERROR
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaProcesado { get; set; }
        public string Error { get; set; }
        public int Intentos { get; set; }
    }
}
