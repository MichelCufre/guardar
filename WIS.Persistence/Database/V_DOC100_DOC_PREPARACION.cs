using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_DOC100_DOC_PREPARACION
    {
        [Key]
        [Column(Order = 1)]
        public int NU_DOCUMENTO_PREPARACION { get; set; }

        public int NU_PREPARACION { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PREPARACION { get; set; }
        public int CD_EMPRESA_EGRESO { get; set; }
        public int CD_EMPRESA_INGRESO { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA_EGRESO { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA_INGRESO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACTIVE { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO { get; set; }
        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO { get; set; }
        [StringLength(50)]
        public string DS_TIPO_DOC_INGRESO { get; set; }
        [StringLength(6)]
        public string ID_ESTADO_DOC_INGRESO { get; set; }
        [StringLength(100)]
        public string DS_ESTADO_DOC_INGRESO { get; set; }       
        [StringLength(1)]
        public string FL_REABRIR_I { get; set; }
        [StringLength(1)]
        public string FL_FINOP_I { get; set; }
        [StringLength(1)]
        public string ESTADO_INICIAL_I { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO_EGRESO { get; set; }
        [StringLength(6)]
        public string TP_DOCUMENTO_EGRESO { get; set; }
        [StringLength(50)]
        public string DS_TIPO_DOC_EGRESO { get; set; }
        [StringLength(6)]
        public string ID_ESTADO_DOC_EGRESO { get; set; }
        [StringLength(100)]
        public string DS_ESTADO_DOC_EGRESO { get; set; }
        [StringLength(1)]
        public string FL_REABRIR_E { get; set; }
        [StringLength(1)]
        public string FL_FINOP_E { get; set; }
        [StringLength(1)]
        public string ESTADO_INICIAL_E { get; set; }

        [StringLength(10)]
        public string TP_OPERATIVA { get; set; }
        [StringLength(100)]
        public string DS_DOMINIO_VALOR { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
