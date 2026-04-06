using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TIPO_MOVIMIENTO")]
    public partial class T_TIPO_MOVIMIENTO
    {
        [Key]
        public short CD_MOVIMIENTO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_MOVIMIENTO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_CATEGORIA { get; set; }
    }
}
