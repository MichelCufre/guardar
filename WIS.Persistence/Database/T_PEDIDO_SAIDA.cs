namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PEDIDO_SAIDA")]
    public partial class T_PEDIDO_SAIDA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_PEDIDO_SAIDA()
        {
            T_DET_PEDIDO_SAIDA = new HashSet<T_DET_PEDIDO_SAIDA>();
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        public int? CD_ROTA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public DateTime? DT_LIBERAR_DESDE { get; set; }

        public DateTime? DT_LIBERAR_HASTA { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANUAL { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        public DateTime? DT_EMITIDO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public short? NU_ORDEN_LIBERACION { get; set; }

        public int? NU_ULT_PREPARACION { get; set; }

        public DateTime? DT_ULT_PREPARACION { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_MEMO_1 { get; set; }

        public int? NU_PREPARACION_MANUAL { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_ORIGEN { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        [Required]
        [StringLength(6)]
        [Column]
        public string CD_CONDICION_LIBERACION { get; set; }

        public int? NU_PREPARACION_PROGRAMADA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [Required]
        [StringLength(6)]
        [Column]
        public string TP_PEDIDO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_EXPEDICION { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_SERIALIZADO_1 { get; set; }

        public int? CD_TRANSPORTADORA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(5)]
        [Column]
        public string CD_UF { get; set; }

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

        [StringLength(20)]
        [Column]
        public string CD_ZONA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_PUNTO_ENTREGA { get; set; }

        public int? NU_ORDEN_ENTREGA { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_COMPARTE_CONTENEDOR_PICKING { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

        public long? NU_INTERFAZ_FACTURACION { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public DateTime? DT_FUN_RESP { get; set; }

        public int? CD_FUN_RESP { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ACTIVIDAD { get; set; }

        public DateTime? DT_GENERICO_1 { get; set; }

        public decimal? NU_GENERICO_1 { get; set; }

        [StringLength(400)]
        [Column]
        public string VL_GENERICO_1 { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SYNC_REALIZADA { get; set; }
        public long? NU_CARGA { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONE { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONE2 { get; set; }

        public decimal? VL_LONGITUD { get; set; }

        public decimal? VL_LATITUD { get; set; }

        public decimal? QT_PONDERACION_PEDIDO { get; set; }

        [StringLength(200)]
        public string VL_PONDERACION_GENERICA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_PEDIDO_SAIDA> T_DET_PEDIDO_SAIDA { get; set; }
    }
}
