using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_INVENTARIO_ENDERECO_DET_ATR")]
    public partial class T_INVENTARIO_ENDERECO_DET_ATR
    {
        [Key]
        public decimal NU_INVENTARIO_ENDERECO_DET { get; set; }

        [Key]
        public int ID_ATRIBUTO { get; set; }

        [StringLength(400)]
        public string VL_ATRIBUTO { get; set; }
    }
}
