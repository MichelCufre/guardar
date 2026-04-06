namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTURAC_PROC_EMP_WFAC005")]
    public partial class V_FACTURAC_PROC_EMP_WFAC005
    {
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }
        
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
        
        [StringLength(200)]
        public string DS_PROCESO { get; set; }
        
        [StringLength(30)]
        public string DS_SITUACAO { get; set; }
        
        [StringLength(1)]
        public string TP_PROCESO { get; set; }
        
        [StringLength(1)]
        public string TP_ULTIMO_PROCESO { get; set; }
        
        public short? CD_SITUACAO_ERROR { get; set; }
        public decimal? QT_RESULTADO { get; set; }
        public DateTime? HR_ULTIMO_PROCESO { get; set; }
    }
}
