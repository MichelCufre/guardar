namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_COF040_IMPRESORAS")]
    public partial class V_COF040_IMPRESORAS
    {
        [Key]
        [StringLength(50)]
        [Column]
        public string CD_IMPRESORA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_IMPRESORA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_LENGUAJE_IMPRESION { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_LENGUAJE_IMPRESION { get; set; }

        public int? CD_SERVIDOR { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_SERVIDOR { get; set; }
    }
}
