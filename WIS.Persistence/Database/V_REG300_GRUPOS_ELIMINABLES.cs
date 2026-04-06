namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_REG300_GRUPOS_ELIMINABLES")]
    public partial class V_REG300_GRUPOS_ELIMINABLES
    {
        [Key]
        [StringLength(50)]
        [Column]
        public string CD_GRUPO { get; set; }
    }
}
