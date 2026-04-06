namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTURA_UND_MEDI_EMP_WFAC230")]
    public class V_FACTURA_UND_MEDI_EMP_WFAC230
    {
        [Key]
        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [StringLength(30)]
        public string DS_UNIDADE_MEDIDA { get; set; }
        
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
        
        [StringLength(30)]
        public string NM_FUNCIONARIO { get; set; }
        
        public int? CD_FUNCIONARIO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
    }
}
