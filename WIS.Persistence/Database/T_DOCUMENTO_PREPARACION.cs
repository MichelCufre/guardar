namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOCUMENTO_PREPARACION")]
    public partial class T_DOCUMENTO_PREPARACION
    {
        [Key]
        [Column(Order = 1)]
        public int NU_DOCUMENTO_PREPARACION { get; set; }
        public int NU_PREPARACION { get; set; }
        public int? CD_EMPRESA_INGRESO { get; set; }
        public int CD_EMPRESA_EGRESO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACTIVE { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO_INGRESO { get; set; }
        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO_INGRESO { get; set; }        
        
        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO_EGRESO { get; set; }
        
        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO_EGRESO { get; set; }        
        
        [StringLength(10)]
        [Column]
        public string TP_OPERATIVA { get; set; }        
        public int? CD_FUNCIONARIO { get; set; }                
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
