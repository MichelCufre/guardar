using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("T_APLICACION_RECORRIDO")]
    public partial class T_APLICACION_RECORRIDO
    {

        [Key]
        public int NU_RECORRIDO { get; set; }

        [Key]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_PREDETERMINADO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

    }
}