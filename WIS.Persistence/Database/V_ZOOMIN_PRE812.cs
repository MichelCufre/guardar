using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_ZOOMIN_PRE812")]
    public partial class V_ZOOMIN_PRE812
    {

        [Key]
        [Column(Order = 0)]
        public int NU_SEQ_PREPARACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [Key]
        [Column(Order = 3)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Key]
        [Column(Order = 6)]
        public decimal CD_FAIXA { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public int? CD_FUNC_PICKEO { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public DateTime? DT_FABRICACAO_PICKEO { get; set; }

        public DateTime? DT_PICKEO { get; set; }

        public int? NU_CONTENEDOR { get; set; }

        [Key]
        [Column(Order = 14)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 15)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 16)]
        public int NU_PREPARACION { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public int? NU_COLA_PICKING { get; set; }

        public short? CD_EQUIPO_ASIGNADO { get; set; }

        public int? CD_FUNC_ASIGNADO { get; set; }

        [StringLength(100)]
        public string NM_FUNC_ASIGNADO { get; set; }

    }
}