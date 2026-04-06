using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO151_STOCK_LPN")]

    public partial class V_STO151_STOCK_LPN
    {
        [Key]
        [Column(Order = 0)]
        public long NU_LPN { get; set; }

        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }
        
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [StringLength(40)]
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

        public decimal? QT_ESTOQUE { get; set; }
        public decimal? QT_RECIBIDA { get; set; }
        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_LPN_BLOQ_CTRL_CAL { get; set; }
        public decimal? QT_LPN_BLOQ_AVERIA { get; set; }
        public decimal? QT_LPN_BLOQ_INVENTARIO { get; set; }
    }
}
