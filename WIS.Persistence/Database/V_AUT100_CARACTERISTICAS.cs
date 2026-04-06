
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_AUT100_CARACTERISTICAS")]
    public partial class V_AUT100_CARACTERISTICAS
    {

        [Key]
        [Column(Order = 0)]
        public decimal NU_AUTOMATISMO_CARACTERISTICA { get; set; }

        public int NU_AUTOMATISMO { get; set; }

        [Required]
        [StringLength(100)]
        public string CD_AUTOMATISMO_CARACTERISTICA { get; set; }

        [Required]
        [StringLength(400)]
        public string VL_AUTOMATISMO_CARACTERISTICA { get; set; }

        [StringLength(400)]
        public string DS_AUTOMATISMO_CARACTERISTICA { get; set; }

        [StringLength(100)]
        public string VL_AUX1 { get; set; }

        public long? NU_AUX1 { get; set; }

        public decimal? QT_AUX1 { get; set; }

        [StringLength(1)]
        public string FL_AUX1 { get; set; }

        [StringLength(4000)]
        public string VL_OPCIONES { get; set; }

    }
}
