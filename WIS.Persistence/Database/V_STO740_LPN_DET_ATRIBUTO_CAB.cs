namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_STO740_LPN_DET_ATRIBUTO_CAB")]
    public partial class V_STO740_LPN_DET_ATRIBUTO_CAB
    {
        [Key]
        public long NU_LPN { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_LPN_EXTERNO { get; set; }

        [Key]
        [StringLength(10)]
        [Column]
        public string TP_LPN_TIPO { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_LPN_TIPO { get; set; }

        [Key]
        public int ID_LPN_DET { get; set; }

        [StringLength(40)]
        [Column]
        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        [Key]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Key]
        public decimal CD_FAIXA { get; set; }

        [Key]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        public int ID_ATRIBUTO { get; set; }

        [StringLength(50)]
        [Column]
        public string NM_ATRIBUTO { get; set; }

        [StringLength(400)]
        [Column]
        public string VL_ATRIBUTO { get; set; }

        [StringLength(400)]
        [Column]
        public string VL_NORMALIZADO { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public decimal QT_ESTOQUE { get; set; }

        public decimal? QT_DECLARADA { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_EXPEDIDA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_INVENTARIO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CTRL_CALIDAD { get; set; }
    }
}