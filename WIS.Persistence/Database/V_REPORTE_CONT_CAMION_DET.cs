
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_REPORTE_CONT_CAMION_DET")]
    public partial class V_REPORTE_CONT_CAMION_DET
    {

        [Key]
        [Column(Order = 0)]
        public int CD_CAMION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string TP_CONTENEDOR { get; set; }

        [Required]
        [StringLength(50)]
        public string DS_TIPO_CONTENEDOR { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [StringLength(3)]
        public string TP_AGENTE { get; set; }

        public decimal? QT_TIPO_CONTENEDOR { get; set; }

    }
}