using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    [Table("V_PRDC_LINEA_KIT175")]
    public class V_PRDC_LINEA_KIT175
    {
        [Key]
        [Column]
        public string CD_PRDC_LINEA { get; set; }
        [Column]
        public string DS_PRDC_LINEA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        [Column]
        public string CD_ENDERECO_ENTRADA { get; set; }
        [Column]
        public string CD_ENDERECO_SALIDA { get; set; }
        [Column]
        public string NU_PRDC_INGRESO { get; set; }
        [Column]
        public string ND_TIPO_LINEA { get; set; }
        [Column]
        public string CD_ENDERECO_BLACKBOX { get; set; }
        [Column]
        public string CD_PRDC_DEFINICION { get; set; }
        [Column]
        public string NM_PRDC_DEFINICION { get; set; }
        public short? CD_SITUACAO { get; set; }
        [Column]
        public string DS_SITUACAO { get; set; }
    }
}
