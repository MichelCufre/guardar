using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_REG700_APLICACION_DISP")]
    public partial class V_REG700_APLICACION_DISP
    {

        [Key]
        public int NU_RECORRIDO { get; set; }

        [Key]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [StringLength(100)]
        public string DS_APLICACION { get; set; }

    }
}
