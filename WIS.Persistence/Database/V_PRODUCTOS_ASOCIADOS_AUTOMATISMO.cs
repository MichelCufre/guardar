
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_PRODUCTOS_ASOCIADOS_AUTOMATISMO")]
    public partial class V_PRODUCTOS_ASOCIADOS_AUTOMATISMO
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string CD_ZONA_UBICACION { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_SEPARACAO { get; set; }

        [StringLength(20)]
        public string CD_UNIDAD_CAJA_AUT { get; set; }

        public int? QT_UNIDAD_CAJA_AUT { get; set; }

        [StringLength(1)]
        public string FL_CONF_CD_BARRAS_AUT { get; set; }
    }
}
