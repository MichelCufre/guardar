
namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_REG015_PRODUTOS_PROVEEDOR")]
    public partial class V_REG015_PRODUTOS_PROVEEDOR
    {
        [Required]
        [Key]
        public int CD_EMPRESA { get; set; }

        [Required]
        [Column]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Column]
        [StringLength(10)]
        [Key]
        public string CD_CLIENTE { get; set; }

        [Column]
        [StringLength(40)]

        public string CD_AGENTE { get; set; }

        [Column]
        [StringLength(3)]

        public string TP_AGENTE { get; set; }

        [Column]
        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [Required]
        [Key]
        [Column]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [Column]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Column]
        [StringLength(30)]
        public string CD_EXTERNO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
