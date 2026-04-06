using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_REPORTE_PACKING_LIST_DET")]
    public partial class V_REPORTE_PACKING_LIST_DET
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
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 5)]
        public int CD_EMPRESA { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public DateTime? DT_FABRICACAO_PICKEO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        public string TP_AGENTE { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

    }
}