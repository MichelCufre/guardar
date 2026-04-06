using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class ProductoCodigoBarra
    {
        public string CodigoBarra { get; set; }

        public int IdEmpresa { get; set; }
        public string IdProducto { get; set; }
        public int IdTipoCodigoBarra { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public decimal? CantEmbalaje { get; set; }
        public short? NumPrioridadeUso { get;set; }
        public long? NumeroTransaccion { get; set; }
        public long? NumeroTransaccionDelete { get; set; }

        public ProductoCodigoBarraTipo TipoCodigoDeBarra { get; set; }
        public Empresa empresa { get; set; }
        public Producto producto { get; set; }
    }
}