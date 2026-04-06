namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_STO720_LPN_LINEAS")]
    public partial class V_STO720_LPN_LINEAS
    {
        public int CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }
        public decimal CD_FAIXA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_LPN_TIPO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public DateTime? DT_ACTIVACION { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_FIN { get; set; }

        [Key]
        public int ID_LPN_DET { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

        [StringLength(40)]
        [Column]
        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        public long NU_LPN { get; set; }
        public long? NU_TRANSACCION { get; set; }
        public decimal QT_ESTOQUE { get; set; }
        public decimal? QT_DECLARADA { get; set; }
        public decimal? QT_RECIBIDA { get; set; }
        public decimal? QT_RESERVA_SAIDA { get; set; }
        public decimal? QT_EXPEDIDA { get; set; }

        [StringLength(40)]
        [Column]
        public string TP_LPN_TIPO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_INVENTARIO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CTRL_CALIDAD { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MOTIVO_AVERIA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(50)]
        public string ID_PACKING { get; set; }

        public int? NU_AGENDA { get; set; }
    }
}