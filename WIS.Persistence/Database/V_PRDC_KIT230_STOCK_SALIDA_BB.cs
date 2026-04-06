using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PRDC_KIT230_STOCK_SALIDA_BB")]
    public partial class V_PRDC_KIT230_STOCK_SALIDA_BB
    {
        [Required]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_TRANSITO_ENTRADA { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        [StringLength(1)]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        public string ID_INVENTARIO { get; set; }

        public DateTime? DT_INVENTARIO { get; set; }

        [StringLength(1)]
        public string ID_CTRL_CALIDAD { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public decimal? QT_MOVIMIENTO_BB { get; set; }

        public decimal? QT_RECHAZO_AVERIA { get; set; }
    }
}
