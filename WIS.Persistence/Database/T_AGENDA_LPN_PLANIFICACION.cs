using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_AGENDA_LPN_PLANIFICACION")]
    public partial class T_AGENDA_LPN_PLANIFICACION
    {
        [Key]
        public int NU_AGENDA { get; set; }

        [Key]
        public long NU_LPN { get; set; }

        [StringLength(1)]
        public string FL_PLANIFICADO { get; set; }

        [StringLength(1)]
        public string FL_RECIBIDO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public int? CD_FUNCIONARIO_RECEPCION { get; set; }

        public DateTime? DT_RECEPCION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
