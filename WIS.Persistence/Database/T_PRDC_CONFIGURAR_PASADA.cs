using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class T_PRDC_CONFIGURAR_PASADA
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

        public virtual T_PRDC_ACCION_INSTANCIA T_PRDC_ACCION_INSTANCIA { get; set; }
        public virtual T_PRDC_DEFINICION T_PRDC_DEFINICION { get; set; }
    }
}
