using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STOCK_PRODUCTO_LPN")]

    public partial class V_STOCK_PRODUCTO_LPN
    {
        [Key]
        [StringLength(40)]
        [Column(Order = 0)]
        public string CD_ENDERECO { get; set; }

        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }

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

        public decimal? QT_ESTOQUE_LPN { get; set; }
        public decimal? QT_RESERVA_LPN { get; set; }
        public decimal? QT_DISPONIBLE_LPN { get; set; }
    }
}
