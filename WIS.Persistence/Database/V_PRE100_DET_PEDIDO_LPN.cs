using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PRE100_DET_PEDIDO_LPN")]
    public partial class V_PRE100_DET_PEDIDO_LPN
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Column(Order = 6)]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [Column]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Column]
        [StringLength(1)]
        public string ID_MANEJO_IDENTIFICADOR { get; set; }

        public decimal? QT_PEDIDO { get; set; }
        public decimal? QT_LIBERADO { get; set; }
        public decimal? QT_ANULADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        [Column]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }
    }
}
