using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public partial class T_PRDC_ACCION_INSTANCIA
    {
        public T_PRDC_ACCION_INSTANCIA()
        {
            this.T_PRDC_CONFIGURAR_PASADA = new HashSet<T_PRDC_CONFIGURAR_PASADA>();
            this.T_PRDC_INGRESO_PASADA = new HashSet<T_PRDC_INGRESO_PASADA>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_ACCION_INSTANCIA { get; set; }
        [Column]
        public string DS_ACCION_INSTANCIA { get; set; }
        [Column]
        public string CD_ACCION { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        [Column]
        public string VL_PARAMETRO1 { get; set; }
        [Column]
        public string VL_PARAMETRO2 { get; set; }
        [Column]
        public string VL_PARAMETRO3 { get; set; }

        public virtual ICollection<T_PRDC_CONFIGURAR_PASADA> T_PRDC_CONFIGURAR_PASADA { get; set; }
        public virtual ICollection<T_PRDC_INGRESO_PASADA> T_PRDC_INGRESO_PASADA { get; set; }
    }
}
