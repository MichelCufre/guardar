namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DOCUMENTO_PROD_DOC290")]
    public partial class V_DOCUMENTO_PROD_DOC290
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

        [StringLength(50)]
        [Column]
        public string DS_TP_DOCUMENTO_EGR { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_TP_DOCUMENTO_ING { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

    }
}
