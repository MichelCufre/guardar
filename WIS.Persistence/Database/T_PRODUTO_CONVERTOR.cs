
namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PRODUTO_CONVERTOR")]
    public partial class T_PRODUTO_CONVERTOR
    {
        [Required]
        [Key]
        public int CD_EMPRESA { get; set; }

        [Column]
        [StringLength(10)]
        [Key]
        public string CD_CLIENTE { get; set; }

        [Required]
        [Key]
        [Column]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Column]
        [StringLength(30)]
        public string CD_EXTERNO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
