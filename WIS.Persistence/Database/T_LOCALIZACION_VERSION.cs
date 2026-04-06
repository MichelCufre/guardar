namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_LOCALIZACION_VERSION")]
    public partial class T_LOCALIZACION_VERSION
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string CD_IDIOMA { get; set; }

        public int NU_VERSION { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
