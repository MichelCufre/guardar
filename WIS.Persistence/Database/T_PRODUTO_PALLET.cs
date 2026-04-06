
namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PRODUTO_PALLET")]
    public partial class T_PRODUTO_PALLET
    {
        [Key]
        [Column]
        [StringLength(20)]
        public string CD_PRODUTO { get; set; }

        [Key]
        public decimal CD_FAIXA { get; set; }
        [Key]
        public int CD_EMPRESA { get; set; }
        [Key]
        public short CD_PALLET { get; set; }

        public decimal QT_UNIDADES { get; set; }

        public short NU_PRIORIDAD { get; set; }
    }
}
