namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_PAR400_MASCARA_FECHA")]
    public partial class V_PAR400_MASCARA_FECHA
    {
        [Key]
        [StringLength(15)]
        [Column]
        public string VL_MASCARA { get; set; }

    }
}
