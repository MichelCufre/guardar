namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_LPARAMETROS_LCON010")]
    public class V_LPARAMETROS_LCON010
    {
        [Key]
        [StringLength(30)]
        [Column]
        public string CD_PARAMETRO { get; set; }

        [Key]
        [StringLength(10)]
        [Column]
        public string DO_ENTIDAD_PARAMETRIZABLE { get; set; }
   
        [StringLength(10)]
        [Column]
        public string DO_VALOR_PARAMETRO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_PARAMETRO { get; set; }

        [StringLength(30)]
        [Column]
        public string VL_PROCEDIMIENTO_VALIDACION { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_EXPRESION_REGULAR_VALIDACIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO { get; set; }

        public short? NU_NIVEL { get; set; }
    }
}
