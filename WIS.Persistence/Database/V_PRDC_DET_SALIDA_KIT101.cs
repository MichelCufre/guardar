using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PRDC_DET_SALIDA_KIT101")]
    public class V_PRDC_DET_SALIDA_KIT101
    {
        [Key]
        public string CD_PRDC_DEFINICION { get; set; }
        
        public int CD_EMPRESA { get; set; }
        
        [Key]        
        public string CD_PRODUTO { get; set; }
        
        
        public decimal CD_FAIXA { get; set; }
        
        public decimal? QT_COMPLETA { get; set; }
        
        public int? QT_PASADA_LINEA { get; set; }
        
        public decimal? QT_CONSUMIDA_LINEA { get; set; }
        
        public string ID_PRODUTO_FINAL { get; set; }
        
        public DateTime? DT_ADDROW { get; set; }
        
        public DateTime? DT_UPDROW { get; set; }
        
        public string NM_EMPRESA { get; set; }
        
        public string DS_PRODUTO { get; set; }
    }
}
