namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REC010_RECEPCION_REFERENCIA")]
    public partial class V_REC010_RECEPCION_REFERENCIA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_REFERENCIA { get; set; }

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

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        public DateTime? DT_VENCIMIENTO_ORDEN { get; set; }

        public DateTime? DT_EMITIDA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_MEMO { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_SERIALIZADO { get; set; }

        public int CD_SITUACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_REFERENCIA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ESTADO_REFERENCIA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO_REFERENCIA { get; set; }
        
        [StringLength(6)]
        [Column]
        public string CD_MONEDA { get; set; }

        [StringLength(80)]
        [Column]
        public string DS_MONEDA { get; set; }
    }
}
