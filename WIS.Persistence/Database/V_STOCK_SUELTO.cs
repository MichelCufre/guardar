using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_STOCK_SUELTO")]
    public partial class V_STOCK_SUELTO
    {
        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        [StringLength(1)]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        public string ID_CTRL_CALIDAD { get; set; }

        [StringLength(1)]
        public string ID_INVENTARIO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_RESERVA_ATRIBUTOS { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }
    }
}
