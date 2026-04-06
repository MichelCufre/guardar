using System;

namespace WIS.Domain.Inventario
{
    public class InventarioUbicacionDetalle
    {
        public InventarioUbicacionDetalle()
        {
            this.InventarioUbicacion = new InventarioUbicacion();
        }

        public decimal Id { get; set; }

        public decimal IdInventarioUbicacion { get; set; }

        public decimal? NuConteoDetalle { get; set; }

        public int? Empresa { get; set; }

        public string Producto { get; set; }

        public string Identificador { get; set; }

        public decimal? CantidadInventario { get; set; }

        public decimal? CantidadDiferencia { get; set; }

        public DateTime? Vencimiento { get; set; }

        public string Estado { get; set; }

        public int? UserId { get; set; }

        public DateTime? FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public decimal? TiempoInsumido { get; set; }

        public string MotivoAjuste { get; set; }

        public long? NuInstanciaConteo { get; set; }

        public decimal? Faixa { get; set; }

        public long? NumeroTransaccion { get; set; }

        public long? NumeroTransaccionDelete { get; set; }

        public long? NumeroLPN { get; set; }

        public int? IdDetalleLPN { get; set; }

        public string ConteoEsperado { get; set; }


        public InventarioUbicacion InventarioUbicacion { get; set; }

    }
}
