namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT100_ESTAN_PEDIDOS_SAIDA")]
    public partial class V_INT100_ESTAN_PEDIDOS_SAIDA
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [StringLength(6)]
        [Column]
        public string CD_CONDICION_LIBERACION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_ORIGEN { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PUNTO_ENTREGA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_ROTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ROTA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_TRANSPORTADORA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_ZONA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO4 { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO_1 { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_ADDROW { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_EMITIDO { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_ENTREGA { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_GENERICO_1 { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_LIBERAR_DESDE { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_LIBERAR_HASTA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MODO_PEDIDO_NRO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_GENERICO_1 { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_PEDIDO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_EXPEDICION { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_PEDIDO { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_COMPARTE_CONTENEDOR_PICKING { get; set; }

        [StringLength(400)]
        [Column]
        public string VL_GENERICO_1 { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_SERIALIZADO_1 { get; set; }
    }
}
