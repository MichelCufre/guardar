namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REC011_REC_REFERENCIA_DET")]
    public partial class V_REC011_REC_REFERENCIA_DET
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_REFERENCIA_DET { get; set; }

        public int NU_RECEPCION_REFERENCIA { get; set; }

        [StringLength(40)]
        [Column]
        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_REFERENCIA { get; set; }

        public decimal? QT_ANULADA { get; set; }

        public decimal? QT_AGENDADA { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_CONFIRMADA_INTERFAZ { get; set; }

        public decimal? IM_UNITARIO { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? QT_SALDO { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string NU_REFERENCIA { get; set; }

        [Required]
        [StringLength(6)]
        [Column]
        public string TP_REFERENCIA { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_REFERENCIA { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ESTADO_REFERENCIA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO_REFERENCIA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public decimal? QT_GENERICO { get; set; }

    }
}
