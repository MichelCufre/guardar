namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_LPARAMETROS_CONFIG_LCON020")]
    public class V_LPARAMETROS_CONFIG_LCON020
    {
        [Key]
        public int NU_PARAMETRO_CONFIGURACION { get; set; }

        [Key]
        [StringLength(30)]
        [Column]
        public string CD_PARAMETRO { get; set; }

        [Key]
        [StringLength(10)]
        [Column]
        public string DO_ENTIDAD_PARAMETRIZABLE { get; set; }
        
        [Key]
        [StringLength(50)]
        [Column]
        public string ND_ENTIDAD { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_INTERNO_WIS { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_DOMINIO_VALOR { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_PARAMETRO { get; set; }
    }
}
