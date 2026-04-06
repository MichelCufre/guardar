namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DET_DOCUMENTO_ACTA")]
    public partial class T_DET_DOCUMENTO_ACTA
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string NU_ACTA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(6)]
        public string TP_ACTA { get; set; }

        public virtual T_DOCUMENTO T_DOCUMENTO { get; set; }

        public virtual T_DOCUMENTO T_DOCUMENTO_INGRESO { get; set; }
    }
}
