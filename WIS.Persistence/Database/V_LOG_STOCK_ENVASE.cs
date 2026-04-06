namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_LOG_STOCK_ENVASE")]
    public partial class V_LOG_STOCK_ENVASE
    {
        [Key]
        public decimal NU_LOG_STOCK_ENVASE { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public DateTime? DT_LOG_ADDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_TRIGGER { get; set; }

        [StringLength(20)]
        [Column]
        public string ID_ENVASE { get; set; }

        [StringLength(10)]
        [Column]
        public string ND_TP_ENVASE { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string DS_TP_ENVASE { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_BARRAS { get; set; }

        [StringLength(10)]
        [Column]
        public string ND_ESTADO_ENVASE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO_ENVASE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ULTIMO_MOVIMIENTO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_OBSERVACIONES { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public DateTime? DT_ULT_RECEPCION { get; set; }

        public DateTime? DT_ULT_CARGA_CAMION { get; set; }

        public DateTime? DT_ULT_EXPEDICION { get; set; }

        public int? CD_USUARIO_ULT_RECEPCION { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_USUARIO_ULT_RECEPCION { get; set; }

        public int? CD_USUARIO_ULT_CARGA_CAMION { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_USUARIO_ULT_CARGA_CAMION { get; set; }

        public int? CD_USUARIO_ULT_EXPEDICION { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_USUARIO_ULT_EXPEDICION { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(500)]
        [Column]
        public string DS_TRANSACCION { get; set; }

        [StringLength(200)]
        [Column]
        public string CD_APLICACION_TRANSACCION { get; set; }

        public int? CD_FUNCIONARIO_TRANSACCION { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_DATA_TRANSACCION { get; set; }

        public DateTime DT_INICIO_TRANSACCION { get; set; }

        public DateTime? DT_FIN_TRANSACCION { get; set; }
    }
}
