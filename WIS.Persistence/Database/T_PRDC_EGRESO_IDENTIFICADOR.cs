using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class T_PRDC_EGRESO_IDENTIFICADOR
    {
        [Key]
        [Column(Order = 0)]
        public string CD_ENDERECO { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CD_PRODUTO { get; set; }
        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }
        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }
        [Key]
        [Column(Order = 4)]
        public long NU_ORDEN { get; set; }
        [Column]
        public string NU_IDENTIFICADOR { get; set; }
        public DateTime? DT_VENCIMIENTO { get; set; }
        public decimal? QT_STOCK { get; set; }
    }
}
