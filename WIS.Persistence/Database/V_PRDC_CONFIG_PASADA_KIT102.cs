using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_PRDC_CONFIG_PASADA_KIT102
    {
        [Key]
        [Column(Order = 0)]
        public string CD_PRDC_DEFINICION { get; set; }
        [Key]
        [Column(Order = 1)]
        public int CD_ACCION_INSTANCIA { get; set; }
        public int QT_PASADAS { get; set; }
        public int NU_ORDEN { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        [Column]
        public string DS_ACCION_INSTANCIA { get; set; }
    }
}
