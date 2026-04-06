namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_LT_DET_DOCUMENTO_EGRESO")]
    public partial class V_LT_DET_DOCUMENTO_EGRESO
    {
        public long? NU_LOG_SECUENCIA { get; set; }

        public DateTime? DT_LOG_ADD_ROW { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_LOG_APLICACION { get; set; }

        public int? CD_LOG_USERID { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_LOG_TRIGGER { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SECUENCIA { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO_INGRESO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO_INGRESO { get; set; }

        public decimal? QT_DESAFECTADA { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? VL_FOB { get; set; }

        public decimal? VL_CIF { get; set; }

        public decimal? QT_DESCARGADA { get; set; }

        [StringLength(64)]
        [Column]
        public string VL_DATO_AUDITORIA { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME { get; set; }
    }
}
