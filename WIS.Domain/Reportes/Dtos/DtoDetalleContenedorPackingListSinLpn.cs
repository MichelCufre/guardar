using System;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtoDetalleContenedorPackingListSinLpn
    {
        public string Pedido { get; set; }

        public string Producto { get; set; }

        public string Descripcion { get; set; }

        public string Identificador { get; set; }

        public DateTime? Vencimiento { get; set; }

        public decimal Cantidad { get; set; }
    }
}
