namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_PRE250_CLIENTES_DISPONIBLES")]
    public partial class V_PRE250_CLIENTES_DISPONIBLES
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [StringLength(51)]
        [Column]
        public string KEY { get; set; }
    }
}
