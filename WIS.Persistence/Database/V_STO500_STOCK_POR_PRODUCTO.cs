using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO500_STOCK_POR_PRODUCTO")]
    public partial class V_STO500_STOCK_POR_PRODUCTO
    {
        [Key]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public decimal? QT_REAL_ESTOQUE { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_TRANSITO_ENTRADA { get; set; }

        public decimal? QT_AVERIADO { get; set; }

        public decimal? QT_LPN { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }

        [StringLength(18)]
        public string DS_ANEXO5 { get; set; }

        public short CD_RAMO_PRODUTO { get; set; }

        [StringLength(200)]
        public string DS_RAMO_PRODUTO { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        [StringLength(100)]
        public string DS_FAMILIA_PRODUTO { get; set; }

        [Key]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

    }
}
