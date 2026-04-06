
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_PRE110_DET_PEDIDO_LPN_ATR")]
    public partial class V_PRE110_DET_PEDIDO_LPN_ATR
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        public string DS_AGENTE { get; set; }

        [Key]
        [Column(Order = 5)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 9)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 10)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 11)]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        public decimal? QT_PENDIENTE { get; set; }

        public decimal? AUXQT_ANULADO { get; set; }

        public decimal QT_PEDIDO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [Key]
        [Column(Order = 17)]
        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [Key]
        [Column(Order = 18)]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [Key]
        [Column(Order = 22)]
        public long NU_DET_PED_SAI_ATRIB { get; set; }
        public string AUXDS_MOTIVO { get; set; }
    }
}