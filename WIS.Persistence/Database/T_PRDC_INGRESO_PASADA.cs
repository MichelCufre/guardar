using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public partial class T_PRDC_INGRESO_PASADA
    {
        [Key]
        [Column(Order = 0)]
        public string NU_PRDC_INGRESO { get; set; }
        [Key]
        [Column(Order = 1)]
        public int QT_PASADAS { get; set; }
        public int? CD_ACCION_INSTANCIA { get; set; }
        [Column]
        public string VL_ACCION_INSTANCIA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public long? NU_TRANSACCION { get; set; }
        [Key]
        [Column(Order = 2)]
        public int NU_ORDEN { get; set; }
        public int? NU_FORMULA_ENSAMBLADA { get; set; }
        [Column]
        public string CD_PRDC_LINEA { get; set; }

        public virtual T_PRDC_ACCION_INSTANCIA T_PRDC_ACCION_INSTANCIA { get; set; }
        public virtual T_PRDC_INGRESO T_PRDC_INGRESO { get; set; }
    }
}
