namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LT_DET_DOCUMENTO")]
    public partial class LT_DET_DOCUMENTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_LOG_SECUENCIA { get; set; }

        public DateTime? DT_LOG_ADD_ROW { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_LOG_APLICACION { get; set; }

        public int? CD_LOG_USERID { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_LOG_TRIGGER { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        public decimal? VL_MERCADERIA { get; set; }

        public decimal? QT_INGRESADA { get; set; }

        public decimal? QT_RESERVADA { get; set; }

        public decimal? QT_DESAFECTADA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_DISPONIBLE { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_DISPONIBLE { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO { get; set; }

        public decimal? VL_CIF { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO_INGRESO { get; set; }

        public decimal? QT_DESCARGADA { get; set; }

        [StringLength(64)]
        [Column]
        public string VL_DATO_AUDITORIA { get; set; }
    }
}
