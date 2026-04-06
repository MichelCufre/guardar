namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_COD_05")]
    public partial class V_COD_05
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        public decimal CD_FAIXA { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [Key]
        [StringLength(10)]
        public string NU_FOTO { get; set; }
        
        [Key]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }
        
        [Key]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [Key]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }
        
        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }
        
        [StringLength(10)]
        public string CD_UNID_EMB { get; set; }

        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
        public decimal? VL_PRECIO_UNITARIO { get; set; }
        public decimal? QT_RESULTADO { get; set; }
        public decimal? QT_TOTAL { get; set; }
        public decimal QT_UND_BULTO { get; set; }
    }
}
