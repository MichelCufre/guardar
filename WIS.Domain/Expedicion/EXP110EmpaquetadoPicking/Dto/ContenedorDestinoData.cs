using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class ContenedorDestinoData
    {
        public int NumeroContenedor { get; set; }
        public int NumeroPreparacion { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CodigoCliente { get; set; }
        public string DescripcionCliente { get; set; }
        public string NumeroPedido { get; set; }
        public string Direccion { get; set; }
        public string CompartContenedorEntrega { get; set; }
        public string TipoPedido { get; set; }
        public string CodigoZona { get; set; }
        public string TipoExpedicion { get; set; }
        public string CodigoRota { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string Anexo4 { get; set; }
        public string Ubicacion { get; set; }
        public string SubClase { get; set; }
        public string TipoContenedor { get; set; }
        public string ClaseEmpaque { get; set; }
        public string IdExternoContenedor { get; set; }
    }
}
