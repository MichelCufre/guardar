namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CAMION")]
    public partial class T_CAMION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CAMION { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_PLACA_CARRO { get; set; }

        public short? CD_ROTA { get; set; }

        public short CD_SITUACAO { get; set; }

        public short? CD_PORTA { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_RESPETA_ORD_CARGA { get; set; }

        public int CD_TRANSPORTADORA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? CD_EMPRESA { get; set; }

        public DateTime? DT_PROGRAMADO { get; set; }
        public DateTime? DT_ARRIBO_CAMION { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_CAMION { get; set; }

        public int? CD_VEICULO { get; set; }

        public DateTime? DT_FACTURACION { get; set; }

        public int? CD_FUNC_CIERRE { get; set; }

        public DateTime? DT_CIERRE { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        public long? NU_INTERFAZ_EJECUCION_FACT { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_TRACKING { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_RUTEO { get; set; }

        public int? NU_ORT_ORDEN { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SYNC_REALIZADA { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ARMADO_EGRESO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HABILITADO_CARGA { get; set; }        
        
        [StringLength(1)]
        [Column]
        public string FL_HABILITADO_ARMADO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HABILITADO_CIERRE { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_DOCUMENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CIERRE_PARCIAL { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_ARMADO_EGRESO { get; set; }
        [StringLength(1)]
        [Column]
        public string FL_AUTO_CARGA { get; set; }
        [StringLength(1)]
        [Column]
        public string FL_CIERRE_AUTO { get; set; }
        [StringLength(1)]
        [Column]
        public string FL_CTRL_CONTENEDORES { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONF_VIAJE_REALIZADA { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_EXTERNO { get; set; }
        public int? CD_EMPRESA_EXTERNA { get; set; }

        public virtual ICollection<T_CLIENTE_CAMION> T_CLIENTE_CAMION { get; set; }
    }
}
