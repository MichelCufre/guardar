namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_STOCK_ENVASE")]
    public partial class T_STOCK_ENVASE
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string ID_ENVASE { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string ND_TP_ENVASE { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_BARRAS { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string ND_ESTADO_ENVASE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ULTIMO_MOVIMIENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_OBSERVACIONES { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        public DateTime? DT_ULT_RECEPCION { get; set; }

        public DateTime? DT_ULT_CARGA_CAMION { get; set; }

        public DateTime? DT_ULT_EXPEDICION { get; set; }

        public int? CD_USUARIO_ULT_RECEPCION { get; set; }

        public int? CD_USUARIO_ULT_CARGA_CAMION { get; set; }

        public int? CD_USUARIO_ULT_EXPEDICION { get; set; }

        public long? NU_TRANSACCION { get; set; }
    }
}
