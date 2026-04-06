using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("T_APLICACION")]
    public partial class T_APLICACION
    {

        [Key]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [StringLength(100)]
        public string DS_APLICACION { get; set; }

        [StringLength(1)]
        public string FL_RECORRIDO { get; set; }

    }
}
