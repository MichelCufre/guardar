
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;

namespace WIS.Persistence.Database
{

    [Table("V_COLA_TRABAJO_PONDERADOR_DET")]
    public partial class V_COLA_TRABAJO_PONDERADOR_DET
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string CD_INST_PONDERADOR { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string CD_PONDERADOR { get; set; }

        [Key]
        [Column(Order = 2)]
        public int NU_COLA_TRABAJO { get; set; }

        public int? NU_PONDERACION { get; set; }

        [StringLength(5)]
        public string VL_OPERACION { get; set; }

        [StringLength(427)]
        public string DS_OPERACION { get; set; }

    }
}