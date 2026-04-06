namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_COF010_LENGUAJE_IMPRESION")]
    public partial class V_COF010_LENGUAJE_IMPRESION
    {
        [Key]
        [StringLength(10)]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CD_LENGUAJE_IMPRESION { get; set; }

        [StringLength(30)]
        [Column(Order = 1)]
        public string DS_LENGUAJE_IMPRESION { get; set; }
    }
}
