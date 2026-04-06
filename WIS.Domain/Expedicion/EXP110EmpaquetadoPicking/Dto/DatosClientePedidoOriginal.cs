using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto
{
    public class DatosClientePedidoOriginal
    {
        public int Empresa { get; set; }
        public string CodigoCliente { get; set; }
        public string DescripcionCliente { get; set; }
        public string NumeroPedido { get; set; }
        public string Direccion { get; set; }
        public string CompartContenedorEntrega { get; set; }
        public string TipoPedido { get; set; }
        public string CodigoZona { get; set; }
        public string TipoExpedicion { get; set; }
        public short CodigoRuta { get; set; }
        public string DescripcionRuta { get; set; }
        public string FechaEntrega { get; set; }
        public string Anexo4 { get; set; }
    }
}
