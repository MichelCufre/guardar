namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_COR_02")]
    public partial class V_COR_02   
    {
        [Key]
        public int NU_PALLET_DET { get; set; }
        
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [StringLength(40)]
        public string NU_PALLET { get; set; }
        
        public int? NU_EJECUCION_FACTURACION { get; set; }
        public int? CD_EMPRESA { get; set; }
        public decimal? VL_PRECIO_UNITARIO { get; set; }
        public decimal? QT_RESULTADO { get; set; }
        public decimal? QT_TOTAL { get; set; }
        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
    }
}
