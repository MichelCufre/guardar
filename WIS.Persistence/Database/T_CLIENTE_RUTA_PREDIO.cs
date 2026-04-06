namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CLIENTE_RUTA_PREDIO")]
    public partial class T_CLIENTE_RUTA_PREDIO
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public short CD_ROTA { get; set; }

        public virtual T_ROTA T_ROTA { get; set; }

        public virtual T_CLIENTE T_CLIENTE { get; set; }
    }
}
