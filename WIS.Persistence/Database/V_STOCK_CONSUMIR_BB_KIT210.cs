using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STOCK_CONSUMIR_BB_KIT210")]
    public partial class V_STOCK_CONSUMIR_BB_KIT210
    {
        [StringLength(10)]
        public string NU_PRDC_INGRESO { get; set; }

        public int? QT_FORMULA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(10)]
        public string CD_PRDC_LINEA { get; set; }

        [StringLength(2)]
        public string NU_PREDIO { get; set; }

        [StringLength(20)]
        public string ND_TIPO_LINEA { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(10)]
        public string CD_PRDC_DEFINICION { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

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

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_TRANSITO_ENTRADA { get; set; }

        [StringLength(1)]
        public string ID_AVERIA { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        [StringLength(1)]
        public string ID_CTRL_CALIDAD { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public decimal? QT_CONSUMIDO { get; set; }
    }
}
