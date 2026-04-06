namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DOCUMENTO_DOC260")]
    public partial class V_DOCUMENTO_DOC260
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOCUMENTO { get; set; }

        [StringLength(2)]
        [Column]
        public string TP_DUA { get; set; }

        [StringLength(25)]
        [Column]
        public string NU_DUA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(200)]
        [Column]
        public string NU_FACTURA { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_DTI { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? NU_AGENDA { get; set; }
    }
}
