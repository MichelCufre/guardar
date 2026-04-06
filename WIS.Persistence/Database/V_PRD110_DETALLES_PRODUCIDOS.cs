
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_PRD110_DETALLES_PRODUCIDOS")]
    public partial class V_PRD110_DETALLES_PRODUCIDOS
    {

        [Key]
        [Column(Order = 0)]
        public long NU_PRDC_SALIDA_REAL { get; set; }

        [StringLength(50)]
        public string NU_PRDC_INGRESO { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [StringLength(100)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_PRODUCIDA { get; set; }

        public decimal? QT_NOTIFICADO { get; set; }

        public int? CD_EMPRESA { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

    }
}