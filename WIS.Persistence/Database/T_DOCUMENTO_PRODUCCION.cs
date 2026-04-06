namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOCUMENTO_PRODUCCION")]
    public partial class T_DOCUMENTO_PRODUCCION
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO_EGR { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO_EGR { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string NU_DOCUMENTO_ING { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(6)]
        public string TP_DOCUMENTO_ING { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string NU_PRDC_INGRESO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public virtual T_DOCUMENTO T_DOCUMENTO_INGRESO { get; set; }

        public virtual T_DOCUMENTO T_DOCUMENTO_EGRESO { get; set; }
    }
}
