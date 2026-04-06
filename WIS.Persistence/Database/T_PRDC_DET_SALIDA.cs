using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    public class T_PRDC_DET_SALIDA
    {
        [Key]
        [Column(Order = 0)]
        public string CD_PRDC_DEFINICION { get; set; }
        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CD_PRODUTO { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }
        public decimal? QT_COMPLETA { get; set; }
        public decimal? QT_INCOMPLETA { get; set; }
        public int? QT_PASADA_LINEA { get; set; }
        public decimal? QT_CONSUMIDA_LINEA { get; set; }
        [Column]
        public string ID_PRODUTO_FINAL { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        public virtual T_PRDC_DEFINICION T_PRDC_DEFINICION { get; set; }
        public virtual T_PRODUTO_FAIXA T_PRODUTO_FAIXA { get; set; }
    }
}
