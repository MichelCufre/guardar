using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_SEG_COLA_TRABAJO_PRE812")]
    public partial class V_SEG_COLA_TRABAJO_PRE812
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_PEDIDO { get; set; }

        [Required]
        [StringLength(6)]
        public string TP_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        public decimal? NU_PUNTUACION { get; set; }

        [StringLength(40)]
        public string USERID { get; set; }

        [StringLength(100)]
        public string FULLNAME { get; set; }

        [StringLength(40)]
        public string CD_EQUIPO { get; set; }

        [StringLength(40)]
        public string DS_EQUIPO { get; set; }

        public decimal? PORC_ASIGNACION { get; set; }

        public decimal? PORC_UNIDADES { get; set; }
        public decimal? QT_ASIGNADO { get; set; }

        [StringLength(8)]
        public string TM_EN_COLA { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

    }
}
