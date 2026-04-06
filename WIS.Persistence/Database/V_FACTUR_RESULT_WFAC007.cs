namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTUR_RESULT_WFAC007")]
    public partial class V_FACTUR_RESULT_WFAC007
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
        
        [StringLength(100)]
        public string DS_SIGNIFICADO { get; set; }
        
        [StringLength(1)]
        public string TP_PROCESO { get; set; }
        
        [StringLength(30)]
        public string DS_SITUACAO { get; set; }
        
        [StringLength(10)]
        public string NU_CUENTA_CONTABLE { get; set; }
        
        [StringLength(100)]
        public string DS_CUENTA_CONTABLE { get; set; }
        
        [StringLength(1000)]
        public string DS_ADICIONAL { get; set; }

        public decimal? QT_RESULTADO { get; set; }
        public short? CD_SITUACAO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
