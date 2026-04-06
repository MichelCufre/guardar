namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTURACION_COR_DET_06")]
    public partial class V_FACTURACION_COR_DET_06
    {
        [Key]
        [StringLength(40)]
        public string NU_PALLET { get; set; }
        
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }
        
        [StringLength(3)]
        public string TP_RESULTADO { get; set; }
        
        public int NU_AGENDA { get; set; }
        public decimal? VL_PRECIO_UNITARIO { get; set; }
        public decimal QT_RESULTADO { get; set; }
        public DateTime DT_FECHA { get; set; }
        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
    }
}
