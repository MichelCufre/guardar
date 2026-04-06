using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_PRDC_PLANIFICACION_PEDIDO")]
    public partial class V_PRDC_PLANIFICACION_PEDIDO
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string NU_PRDC_INGRESO { get; set; }

        [Key]
        [Column(Order = 1)]
        public int? CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal? CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal? QT_PENDIENTE { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }
        public decimal? QT_PEDIR { get; set; }

        public decimal? QT_TEORICO { get; set; }

    }
}