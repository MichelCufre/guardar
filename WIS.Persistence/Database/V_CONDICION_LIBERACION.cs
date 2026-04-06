namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_CONDICION_LIBERACION")]
    public partial class V_CONDICION_LIBERACION
    {
        [Key]
        [StringLength(6)]
        [Column]
        public string CD_CONDICION_LIBERACION { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_CONDICION_LIBERACION { get; set; }
    }
}
