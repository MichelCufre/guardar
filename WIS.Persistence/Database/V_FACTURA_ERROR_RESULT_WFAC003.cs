namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTURA_ERROR_RESULT_WFAC003")]
    public partial class V_FACTURA_ERROR_RESULT_WFAC003
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        public int NU_LINEA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
       
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
        
        [StringLength(200)]
        public string DS_PROBLEMA { get; set; }
        
        public short? CD_ERROR { get; set; }
    }
}
