namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_COD_06")]
    public partial class V_COD_06
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [Key]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }
        
        [Key]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }
        
        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
        public DateTime? DT_EXPEDIDO { get; set; }
        public decimal? VL_PRECIO_UNITARIO { get; set; }
        public decimal? QT_RESULTADO { get; set; }
        public decimal? QT_TOTAL { get; set; }
    }
}
