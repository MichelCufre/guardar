using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_LPARAMETRO_CONFIGURACION")]
    public partial class T_LPARAMETRO_CONFIGURACION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PARAMETRO_CONFIGURACION { get; set; }

        [StringLength(30)]
        public string CD_PARAMETRO { get; set; }

        [StringLength(10)]
        public string DO_ENTIDAD_PARAMETRIZABLE { get; set; }

        [StringLength(50)]
        public string ND_ENTIDAD { get; set; }

        [StringLength(4000)]
        public string VL_PARAMETRO { get; set; }

        public virtual T_LPARAMETRO_NIVEL T_LPARAMETRO_NIVEL { get; set; }
    }
}
