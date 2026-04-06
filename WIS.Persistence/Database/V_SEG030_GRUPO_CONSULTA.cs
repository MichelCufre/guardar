namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_SEG030_GRUPO_CONSULTA")]
    public partial class V_SEG030_GRUPO_CONSULTA
    {
        [Key]
        [StringLength(20)]
        [Column]
        public string CD_GRUPO_CONSULTA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_GRUPO_CONSULTA { get; set; }
    }
}
