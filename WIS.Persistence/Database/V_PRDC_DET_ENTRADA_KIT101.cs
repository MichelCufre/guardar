using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    [Table("V_PRDC_DET_ENTRADA_KIT101")]
    public partial class V_PRDC_DET_ENTRADA_KIT101
    {
        [Key]
        [Column(Order = 0)]
        public string CD_PRDC_DEFINICION { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CD_COMPONENTE { get; set; }
        [Key]
        [Column(Order = 2)]
        public short NU_PRIORIDAD { get; set; }        
        public int CD_EMPRESA { get; set; }
        [Column]
        public string CD_PRODUTO { get; set; }
        public decimal CD_FAIXA { get; set; }        
        public decimal? QT_COMPLETA { get; set; }
        public int? QT_PASADA_LINEA { get; set; }
        public decimal? QT_CONSUMIDA_LINEA { get; set; }
        public int? CD_EMPRESA_PEDIDO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        [Column]
        public string NM_EMPRESA { get; set; }
        [Column]
        public string DS_PRODUTO { get; set; }
        [Column]
        public string NM_EMPRESA_PEDIDO { get; set; }
    }
}
