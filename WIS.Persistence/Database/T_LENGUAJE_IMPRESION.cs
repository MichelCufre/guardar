namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_LENGUAJE_IMPRESION")]
    public partial class T_LENGUAJE_IMPRESION
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string CD_LENGUAJE_IMPRESION { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_LENGUAJE_IMPRESION { get; set; }

        public virtual ICollection<T_IMPRESORA> T_IMPRESORA { get; set; }
    }
}
