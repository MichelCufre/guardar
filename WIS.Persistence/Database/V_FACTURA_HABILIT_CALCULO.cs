using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_FACTURA_HABILIT_CALCULO
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        public int? CD_FUNC_EJECUCION { get; set; }
        public int? CD_FUNC_PROGRAMACION { get; set; }
        public int? CD_FUNC_APROBACION { get; set; }
        public short? CD_SITUACAO { get; set; }
        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
        public DateTime? DT_CORTE_QUINCENA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_PROGRAMACION { get; set; }
        public DateTime? DT_EJECUCION { get; set; }
        public DateTime? DT_APROBACION { get; set; }
    }
}
