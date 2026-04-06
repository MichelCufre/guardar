using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_CAMION_CTRL_CONTENEDORES")]
    public partial class V_CAMION_CTRL_CONTENEDORES
    {
        [Key]
        public int CD_CAMION { get; set; }

        [Key]
        public long NU_CARGA { get; set; }

        [Key]
        public int NU_CONTENEDOR { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_CONTROL { get; set; }
    }
}
