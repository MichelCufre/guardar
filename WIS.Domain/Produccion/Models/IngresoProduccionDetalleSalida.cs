using System;

namespace WIS.Domain.Produccion.Models
{
    public class IngresoProduccionDetalleSalida
    {
        public long NuPrdcIngresoSalida { get; set; }

        public string NuPrdcIngreso { get; set; }

        public long? NuPrdcIngresoTeorico { get; set; }

        public string Producto { get; set; }

        public int? Empresa { get; set; }

        public decimal? Faixa { get; set; }

        public decimal? QtProducido { get; set; }

        public decimal? QtNotificado { get; set; }

        public string Identificador { get; set; }

        public string NdMotivo { get; set; }

        public string DsMotivo { get; set; }

        public long? NuTransaccion { get; set; }

        public DateTime? DtVencimiento { get; set; }

        public DateTime? DtAddrow { get; set; }

        public long? NuOrden { get; set; }

        public string NdEstado { get; set; }

        public string DsAnexo1 { get; set; }

        public string DsAnexo2 { get; set; }

        public string DsAnexo3 { get; set; }

        public string DsAnexo4 { get; set; }

        public decimal? CantidadTeorica { get; set; }
    }
}
