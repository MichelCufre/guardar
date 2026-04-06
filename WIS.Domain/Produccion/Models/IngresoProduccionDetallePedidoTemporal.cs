using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Produccion.Models
{
    public class IngresoProduccionDetallePedidoTemporal
    {
        public long? Id { get; set; }

        public string IdIngreso { get; set; }

        public string Producto { get; set; }

        public string Lote { get; set; }

        public int Empresa { get; set; }

        public decimal Faixa { get; set; }

        public decimal? CantidadAPedir { get; set; }

        public long? nuTransaccion { get; set; }

        public long? nuTransaccionDelete { get; set; }

        public DateTime? dtAddrow { get; set; }
        public decimal? CantidadReserva { get; set; }
        public decimal? CantidadReservar { get; set; }
        public string Ubicacion { get; set; }
        public decimal? CantidadPendiente { get; set; }
    }
}
