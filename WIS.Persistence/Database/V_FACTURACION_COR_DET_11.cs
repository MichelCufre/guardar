namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTURACION_COR_DET_11")]
    public partial class V_FACTURACION_COR_DET_11
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }
        
        [Key]
        [StringLength(3)]
        public string TP_RESULTADO { get; set; }
        
        [Key]
        public DateTime DT_FECHA { get; set; }
        
        [Key]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }
        
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }
        
        [StringLength(50)]
        public string CD_ENVASE { get; set; }
        
        [StringLength(10)]
        public string CD_UNID_EMB { get; set; }
        
        public int? NU_AGENDA { get; set; }
        public decimal QT_BULTOS { get; set; }
        public decimal? VL_PRECIO_UNITARIO { get; set; }
        public decimal QT_RESULTADO { get; set; }
        public decimal? QT_TOTAL { get; set; }
        public DateTime DT_CIERRE { get; set; }
    }
}
