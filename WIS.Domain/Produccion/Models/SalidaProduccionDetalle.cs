using System;

namespace WIS.Domain.Produccion.Models
{
    public class SalidaProduccionDetalle
    {
        public long Id { get; set; }

        public string NuPrdcIngreso { get; set; }

        public string Ubicacion { get; set; }

        public int Empresa { get; set; }

        public string Producto { get; set; }

        public decimal Faixa { get; set; }

        public string Identificador { get; set; }

        public DateTime? Vencimiento { get; set; }

        public decimal Cantidad { get; set; }

        public long NuTransaccion { get; set; }

        public string Motivo { get; set; }

        public DateTime FechaAlta { get; set; }

        public string FlPendienteNotificar { get; set; }

        public string DsMotivo { get; set; }

        public decimal CantidadTeorica { get; set; }

        public long NuPrdcIngresoSalida { get; set; }
    }
}
