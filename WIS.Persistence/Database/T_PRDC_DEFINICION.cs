using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Persistence.Database;

namespace WIS.Persistence.Database
{
    public class T_PRDC_DEFINICION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_PRDC_DEFINICION()
        {
            this.T_PRDC_CONFIGURAR_PASADA = new HashSet<T_PRDC_CONFIGURAR_PASADA>();
            this.T_PRDC_DET_ENTRADA = new HashSet<T_PRDC_DET_ENTRADA>();
            this.T_PRDC_DET_SALIDA = new HashSet<T_PRDC_DET_SALIDA>();
            this.T_PRDC_INGRESO = new HashSet<T_PRDC_INGRESO>();
        }

        [Key]
        [Column]
        public string CD_PRDC_DEFINICION { get; set; }
        [Column]
        public string NM_PRDC_DEFINICION { get; set; }
        [Column]
        public string DS_PRDC_DEFINICION { get; set; }
        public int CD_EMPRESA { get; set; }
        public short? CD_SITUACAO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        [Column]
        public string TP_PRDC_DEFINICION { get; set; }
        public int? QT_PASADAS_POR_FORMULA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual T_EMPRESA T_EMPRESA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual T_SITUACAO T_SITUACAO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRDC_CONFIGURAR_PASADA> T_PRDC_CONFIGURAR_PASADA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRDC_DET_ENTRADA> T_PRDC_DET_ENTRADA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRDC_DET_SALIDA> T_PRDC_DET_SALIDA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRDC_INGRESO> T_PRDC_INGRESO { get; set; }
    }
}
