using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class CodigoBarrasResponse
    {
        [StringLength(50)]
        public string Codigo { get; set; }
        public int Empresa { get; set; }
        [StringLength(40)]
        public string Producto { get; set; }
        public int? TipoCodigo { get; set; }
        public short? PrioridadUso { get; set; }
        public decimal? CantidadEmbalaje { get; set; }
        public string FechaInsercion { get; set; }
        public string FechaModificacion { get; set; }
    }
}
