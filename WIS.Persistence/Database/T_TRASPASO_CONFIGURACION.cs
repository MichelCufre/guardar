using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_TRASPASO_CONFIGURACION")]
    public partial class T_TRASPASO_CONFIGURACION
    {
        public long NU_TRASPASO_CONFIGURACION { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_TODA_EMPRESA { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_TODO_TIPO_TRASPASO { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_GENERACION_AUTO_CABEZAL { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_REPLICA_PRODUCTOS { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_REPLICA_CODIGOS_BARRAS { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_CTRL_CARACT_IGUALES { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_REPLICA_AGENTES { get; set; }


        [StringLength(6)]
        public string CD_TIPO_DOCUMENTO_INGRESO { get; set; }


        [StringLength(6)]
        public string CD_TIPO_DOCUMENTO_EGRESO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
