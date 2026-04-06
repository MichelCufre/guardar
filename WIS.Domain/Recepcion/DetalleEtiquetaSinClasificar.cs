using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class DetalleEtiquetaSinClasificar
    {
        public int EtiquetaLote { get; set; }
        public int Agenda { get; set; }
        public string EtiquetaExterna { get; set; }
        public string TipoEtiqueta { get; set; }
        public string CodigoProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public string Lote { get; set; }
        public decimal Faixa { get; set; }
        public int CodigoEmpresa { get; set; }
        public string DescEmpresa { get; set; }
        public decimal? Cantidad { get; set; }
        public string Ubicacion { get; set; }
    }
}
