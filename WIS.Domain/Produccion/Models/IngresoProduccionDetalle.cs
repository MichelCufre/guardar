using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Produccion.Models
{
    public class IngresoProduccionDetalle
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

        public DateTime FechaAlta { get; set; }

        public long NuTransaccion { get; set; }

        public string Motivo { get; set; }

        public string FlPendienteNotificar { get; set; }

        public long NuPrdcIngresoReal { get; set; }

        public decimal QtRealOriginal { get; set; }

        public string Referencia { get; set; }

        public bool UsarSoloReserva { get; set; }
    }
}
