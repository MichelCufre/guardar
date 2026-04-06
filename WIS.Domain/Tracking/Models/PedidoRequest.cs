using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PedidoRequest
    {
        public string Numero { get; set; }
        public int CodigoEmpresa { get; set; }
        public string TipoAgente { get; set; }
        public string CodigoAgente { get; set; }
        public string FechaEmitido { get; set; }
        public string FechaRecibido { get; set; }
        public string FechaEntrega { get; set; }
        public string Memo { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Anexo4 { get; set; }
    }
}
