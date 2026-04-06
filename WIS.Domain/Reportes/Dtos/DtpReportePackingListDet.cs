using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtpReportePackingListDet
    {
        public int Camion { get; set; }
        public string Cliente { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Lote { get; set; }
        public int Empresa { get; set; }
        public decimal? CantidadProducto { get; set; }
        public DateTime? Vencimiento { get; set; }
        public string DescripcionProducto { get; set; }
        public string DescripcionCliente { get; set; }
        public string Agente { get; set; }
        public string TipoCliente { get; set; }
        public string NombreEmpresa { get; set; }
    }
}
