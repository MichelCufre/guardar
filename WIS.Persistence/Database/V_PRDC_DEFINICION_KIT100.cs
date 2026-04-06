using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    [Table("V_PRDC_DEFINICION_KIT100")]
    public partial class V_PRDC_DEFINICION_KIT100
    {
        [Key]
        [Column(Order = 0)]
        public string CD_PRDC_DEFINICION { get; set; }
        [Column]
        public string NM_PRDC_DEFINICION { get; set; }
        [Column]
        public string DS_PRDC_DEFINICION { get; set; }
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }
        public short? CD_SITUACAO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        [Column]
        public string NM_EMPRESA { get; set; }
        [Column]
        public string DS_SITUACAO { get; set; }
        [Column]
        public string ACTIVO { get; set; }
        public int? QT_PASADAS_POR_FORMULA { get; set; }
    }
}
